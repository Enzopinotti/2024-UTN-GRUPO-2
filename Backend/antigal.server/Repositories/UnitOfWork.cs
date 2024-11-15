using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using antigal.server.Data;
using antigal.server.Mapping;

namespace antigal.server.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly CarritoMapper _carritoMapper; // Nuevo campo

        private IOrderRepository? _orderRepository;
        private ISaleRepository? _saleRepository;
        private IProductRepository? _productRepository;
        private ICategoriaRepository? _categoriaRepository;
        private IProductCategoryRepository? _productCategoryRepository;
        private ICartRepository? _cartRepository;
        private readonly IEnvioRepository _envioRepository; // Cambiado a no nullable y readonly
        // Agrega otros repositorios según sea necesario

        // Modificar el constructor para aceptar CarritoMapper y IEnvioRepository
        public UnitOfWork(AppDbContext context, CarritoMapper carritoMapper, IEnvioRepository envioRepository)
        {
            _context = context;
            _carritoMapper = carritoMapper;
            _envioRepository = envioRepository;
        }

        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);
        public ISaleRepository Sales => _saleRepository ??= new SaleRepository(_context);
        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public ICategoriaRepository Categories => _categoriaRepository ??= new CategoriaRepository(_context);
        public IProductCategoryRepository ProductCategories => _productCategoryRepository ??= new ProductCategoryRepository(_context);
        public ICartRepository Carts => _cartRepository ??= new CartRepository(_context, _carritoMapper);
        public IEnvioRepository Envio => _envioRepository; // Implementación de la propiedad Envio

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}