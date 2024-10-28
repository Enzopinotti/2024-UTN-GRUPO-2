using antigal.server.Models;
using FluentValidation;

namespace antigal.server.Validaciones
{
    public class ValidacionCategoria: AbstractValidator<Categoria>
    {
        public ValidacionCategoria()
        {
            //nombre
            RuleFor(c => c.nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres");

            //descripcion
            RuleFor(c => c.descripcion)
                .MaximumLength(500).WithMessage("La descripcion puede tener 500 caracteres como maximo");

            //imagen
            //RuleFor(c => c.imagen).

        }

    }
}
