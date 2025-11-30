using FluentValidation;
using UHO_API.Models;

namespace UHO_API.Features.Area.Validations;

public class AreaModelValidator:AbstractValidator<AreaModel>
{
    public AreaModelValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre del Area no puede estar vacio")
            .NotNull().WithMessage("El nombre del Area no puede ser nulo.");
    }
}