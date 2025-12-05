using FluentValidation;
using UHO_API.Features.Users.Commands;

namespace UHO_API.Features.Users.Validations;

public class DegradeUserValidator : AbstractValidator<DegradeUserToUsuarioNormalCommand>
{
    public DegradeUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es requerido.");
    }
}