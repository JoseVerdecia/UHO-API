using FluentValidation;
using UHO_API.Features.Authentication.Queries;

namespace UHO_API.Features.Authentication.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("El email es requerido")
            .NotEmpty().WithMessage("El email no puede estar vacio")
            .EmailAddress().WithMessage("El email no esta en un formato valido");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("La contraseña es requerida")
            .NotEmpty().WithMessage("La contraseña no puede estar vacia")
            .MinimumLength(6).WithMessage("La contraseña debe tener 6 o mas caracteres");

        RuleFor(x => x.FullName)
            .NotNull().WithMessage("El nombre es requerido")
            .NotEmpty().WithMessage("El nombre no puede estar vacio");
    }
}