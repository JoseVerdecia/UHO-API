using FluentValidation;

namespace UHO_API.Features.Users.Validations;

public class DegradeUserValidator : AbstractValidator<DegradeUserToUsuarioNormalCommand>
{
    public DegradeUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es requerido.");
    }
}