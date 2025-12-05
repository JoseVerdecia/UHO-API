using FluentValidation;
using UHO_API.Features.Authentication.Queries;

namespace UHO_API.Features.Authentication.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no esta en un formato valido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}