using ecoaccion.Core.DTOs.Admin;
using FluentValidation;

namespace ecoaccion.Application.Validator.Auth
{
    public class AdminValidator:AbstractValidator<AdminInsertDto>
    {
        public AdminValidator() {
            RuleFor(x => x.Correo)
                   .NotEmpty().WithMessage("El correo es obligatorio.")
                   .EmailAddress().WithMessage("El correo no es válido.")
                   .MaximumLength(150).WithMessage("El correo no debe superar los 150 caracteres.")
                   .Matches(@"^[\w\.\-]+@([\w\-]+\.)+[a-zA-Z]{2,}$").WithMessage("El correo debe tener un formato válido.");

            RuleFor(x => x.NombreUsuario)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre de usuario no debe superar los 100 caracteres.")
                .Matches(@"^[a-zA-Z0-9_\-\.]+$").WithMessage("El nombre de usuario solo puede contener letras, números, guiones, puntos y guiones bajos.");

            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(200).WithMessage("La contraseña no debe superar los 200 caracteres.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d\W]{6,}$").WithMessage("La contraseña debe contener al menos una letra y un número.");

            RuleFor(x => x.ConfirmarContraseña)
                .Equal(x => x.Contraseña)
                .WithMessage("Las contraseñas no coinciden.");
        }
    }
}
