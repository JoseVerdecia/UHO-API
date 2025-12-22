using FluentValidation;
using UHO_API.Core.Entities;
using UHO_API.Features.Area.Commands;


namespace UHO_API.Features.Area.Validations;

public class AreaModelValidator:AbstractValidator<CreateAreaCommand>
{
    public AreaModelValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del Area no puede estar vacio")
            .NotNull().WithMessage("El nombre del Area no puede ser nulo.")
            .MaximumLength(100).WithMessage("El nombre solo puede tener 100 caracteres");
    }
}