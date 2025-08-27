using ecoaccion.Core.DTOs.Desafio;
using FluentValidation;

namespace ecoaccion.Application.Validator.Desafios
{
    public class DesafioInsertValidator:AbstractValidator<DesafioInsertDto>
    {
        public DesafioInsertValidator() 
        {
            RuleFor(x => x.Titulo)
               .NotEmpty().WithMessage("El título es obligatorio.")
               .MaximumLength(100).WithMessage("El título no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.Meta)
                .NotEmpty().WithMessage("La meta es obligatoria.")
                .MaximumLength(200).WithMessage("La meta no puede superar los 200 caracteres.");

            RuleFor(x => x.IdAdmin)
                .NotNull().WithMessage("El IdAdmin es obligatorio.")
                .InclusiveBetween(int.MinValue, int.MaxValue).WithMessage("El IdAdmin debe ser un número entero válido.")
                .GreaterThan(0).WithMessage("El IdAdmin debe ser mayor que cero.");

            RuleFor(x => x.FechaInicio)
              .NotEqual(DateTime.MinValue).WithMessage("La fecha de inicio es obligatoria y debe ser válida.");

            RuleFor(x => x.FechaFin)
                .NotEqual(DateTime.MinValue).WithMessage("La fecha de fin es obligatoria y debe ser válida.")
                .GreaterThanOrEqualTo(x => x.FechaInicio).WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");
        }
    }
}
