using System.Data;
using FluentValidation;
using UHO_API.Core.Entities;
using UHO_API.Features.Proceso.Commands;

namespace UHO_API.Features.Proceso.Validations;

public class ProcesoValidations :AbstractValidator<CreateProcesoCommand>
{
    public ProcesoValidations()
    {
        RuleFor(x => x.nombre)
            .NotNull().WithMessage("El nombre del Proceso no puede ser nulo.")
            .NotEmpty().WithMessage("El nombre del Proceso no puede estar vacio")
            .MaximumLength(50).WithMessage("El nombre del Proceso no debe ser mayor a 50 caracteres");
    }
}