from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROGRAM = ROOT / "Backend" / "antigal.server" / "Program.cs"
UNIT_OF_WORK = ROOT / "Backend" / "antigal.server" / "Repositories" / "UnitOfWork.cs"
IUNIT_OF_WORK = ROOT / "Backend" / "antigal.server" / "Repositories" / "IUnitOfWork.cs"
PRODUCT_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "ProductService.cs"
LIKE_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "LikeService.cs"
CONTACTO_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "ContactoService.cs"
PAYMENT_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "PaymentService.cs"
ENVIO_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "EnvioService.cs"

failures: list[str] = []

program = PROGRAM.read_text(encoding="utf-8-sig")
unit = UNIT_OF_WORK.read_text(encoding="utf-8-sig")
iunit = IUNIT_OF_WORK.read_text(encoding="utf-8-sig")
product_service = PRODUCT_SERVICE.read_text(encoding="utf-8-sig")
like_service = LIKE_SERVICE.read_text(encoding="utf-8-sig")
contacto_service = CONTACTO_SERVICE.read_text(encoding="utf-8-sig")
payment_service = PAYMENT_SERVICE.read_text(encoding="utf-8-sig")
envio_service = ENVIO_SERVICE.read_text(encoding="utf-8-sig")

unit_owned = {
    "IProductRepository": (
        "builder.Services.AddScoped<IProductRepository, ProductRepository>();",
        "public IProductRepository Products => _productRepository ??= new ProductRepository(_context);",
        "IProductRepository Products { get; }",
    ),
    "ICategoriaRepository": (
        "builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();",
        "public ICategoriaRepository Categories => _categoriaRepository ??= new CategoriaRepository(_context);",
        "ICategoriaRepository Categories { get; }",
    ),
    "IProductCategoryRepository": (
        "builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();",
        "public IProductCategoryRepository ProductCategories => _productCategoryRepository ??= new ProductCategoryRepository(_context);",
        "IProductCategoryRepository ProductCategories { get; }",
    ),
    "ICartRepository": (
        "builder.Services.AddScoped<ICartRepository, CartRepository>();",
        "public ICartRepository Carts => _cartRepository ??= new CartRepository(_context, _carritoMapper);",
        "ICartRepository Carts { get; }",
    ),
    "IOrderRepository": (
        "builder.Services.AddScoped<IOrderRepository, OrderRepository>();",
        "public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);",
        "IOrderRepository Orders { get; }",
    ),
    "ILikeRepository": (
        "builder.Services.AddScoped<ILikeRepository, LikeRepository>();",
        "public ILikeRepository Likes => _likeRepository ??= new LikeRepository(_context);",
        "ILikeRepository Likes { get; }",
    ),
    "IContactoRepository": (
        "builder.Services.AddScoped<IContactoRepository, ContactoRepository>();",
        "public IContactoRepository Contactos => _contactoRepository ??= new ContactoRepository(_context);",
        "IContactoRepository Contactos { get; }",
    ),
}

for service, (registration, owner_contract, interface_contract) in unit_owned.items():
    if registration in program:
        failures.append(f"redundant direct DI registration returned: {service}")
    if owner_contract not in unit:
        failures.append(f"UnitOfWork ownership contract missing: {service}")
    if interface_contract not in iunit:
        failures.append(f"IUnitOfWork surface missing: {service}")

# These direct repository registrations remain intentional because maintained
# services consume them directly outside UnitOfWork.
required_direct = {
    "IPaymentRepository": (
        "builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();",
        "IPaymentRepository paymentRepository",
        payment_service,
    ),
    "IEnvioRepository": (
        "builder.Services.AddScoped<IEnvioRepository, EnvioRepository>();",
        "IEnvioRepository envioRepository",
        unit + "\n" + envio_service,
    ),
}

for service, (registration, consumer_contract, consumer_source) in required_direct.items():
    if registration not in program:
        failures.append(f"required direct DI registration missing: {service}")
    if consumer_contract not in consumer_source:
        failures.append(f"direct DI consumer contract missing: {service}")

# SaleRepository is also UnitOfWork-owned and has never needed a direct DI pin.
if "AddScoped<ISaleRepository" in program:
    failures.append("ISaleRepository should remain UnitOfWork-owned, not directly registered")
if "public ISaleRepository Sales => _saleRepository ??= new SaleRepository(_context);" not in unit:
    failures.append("UnitOfWork SaleRepository ownership contract missing")

if "IProductRepository productRepository" in product_service:
    failures.append("ProductService regained a direct IProductRepository constructor dependency")
if "_productRepository" in product_service:
    failures.append("ProductService regained direct product repository state")
for contract in (
    "_unitOfWork.Products.GetProductsAsync(orden, precio)",
    "_unitOfWork.Products.GetFeaturedProductsAsync()",
):
    if contract not in product_service:
        failures.append(f"ProductService UnitOfWork product path missing: {contract}")

print("unitofwork-owned-direct-registration-count=0")
if "AppDbContext" in like_service or "_context" in like_service:
    failures.append("LikeService regained direct AppDbContext state")
for contract in (
    "_unitOfWork.Likes.AddLikeAsync(userId, productoId)",
    "_unitOfWork.Likes.RemoveLikeAsync(userId, productoId)",
    "_unitOfWork.Likes.GetUserLikesAsync(userId)",
):
    if contract not in like_service:
        failures.append(f"LikeService UnitOfWork repository path missing: {contract}")

if "AppDbContext" in contacto_service or "_context" in contacto_service:
    failures.append("ContactoService regained direct AppDbContext state")
for contract in (
    "_unitOfWork.Contactos.AddAsync(contacto)",
    "_unitOfWork.Contactos.GetAllAsync()",
    "_unitOfWork.Contactos.GetByIdAsync(id)",
):
    if contract not in contacto_service:
        failures.append(f"ContactoService UnitOfWork repository path missing: {contract}")
if "builder.Services.AddScoped<IContactoService, ContactoService>();" not in program:
    failures.append("ContactoService DI registration missing")

print("unitofwork-owned-repositories=Products,Orders,Sales,Categories,ProductCategories,Carts,Likes,Contactos")
print("productservice-product-repository-authority=IUnitOfWork.Products")
print("likeservice-like-repository-authority=IUnitOfWork.Likes")
print("contactoservice-contacto-repository-authority=IUnitOfWork.Contactos")
print("direct-repository-di=IPaymentRepository,IEnvioRepository")

if failures:
    print("Backend repository DI authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-repository-di-authority=clean")
