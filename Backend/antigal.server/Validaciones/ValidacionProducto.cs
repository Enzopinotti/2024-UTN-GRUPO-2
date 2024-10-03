using antigal.server.Models;
using FluentValidation;

namespace antigal.server.Validaciones
{
    public class ValidacionProducto: AbstractValidator<Producto>
    {
        public ValidacionProducto()
        {
            RuleFor(p => p.nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres");

            RuleFor(p => p.marca)
                .NotEmpty().WithMessage("La marca es obligatoria");

            RuleFor(p => p.descripcion)
                .MaximumLength(500).WithMessage("La descripción puede tener 500 caracteres como maximo");

            /*
            RuleFor(p => p.codigoBarras)
                .Matches(@"^\d{12,13}$").WithMessage("El código de barras debe contener 12 o 13 dígitos");
            */

            RuleFor(p => p.precio)
                .NotEmpty().WithMessage("El precio es obligatorio")
                .GreaterThan(0).WithMessage("El precio debe ser mayor que cero");

            RuleFor(p => p.stock)
                .NotEmpty().WithMessage("El stock es obligatorio")
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad en stock no puede ser negativa");
        }
    }
}
