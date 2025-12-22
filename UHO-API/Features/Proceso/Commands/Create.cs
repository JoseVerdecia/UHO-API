using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Commands;

public record CreateProcesoCommand(string nombre, string? jefeProcesoId): IRequest<ProcesoDto>;


public class CreateProcesoHandler : IRequestHandler<CreateProcesoCommand, ProcesoDto>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly IValidator<CreateProcesoCommand> _validator;

    public CreateProcesoHandler(IUnitOfWorks uow, UserManager<ApplicationUser> userManager, IRoleChangesService roleChangesService,IValidator<CreateProcesoCommand> validator)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _validator = validator;
    }

    public async Task<Result<ProcesoDto>> Handle(CreateProcesoCommand request, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(request,cancellationToken);

        if (!resultValidation.IsValid)
        {
            var errorMessages = string.Join(", ", resultValidation.Errors.Select(e => e.ErrorMessage));

            return Result.Failure<ProcesoDto>(
                Error.Validation("Proceso.Invalido", errorMessages) // Puedes usar un código más específico
            );
        }
        if (string.IsNullOrWhiteSpace(request.nombre))
        {
            return Result.Failure<ProcesoDto>(
                Error.Validation("Nombre", "El nombre del proceso es requerido")
            );
        }
        
        var existingProceso = await _uow.Area.Get(a => a.Nombre == request.nombre);
        
        if (existingProceso is not null)
        {
            return Result.Failure<ProcesoDto>(
                Error.Conflict("Proceso", "Nombre", request.nombre)
            );
        }

        ApplicationUser? jefeProceso = null;
        
        if (!string.IsNullOrWhiteSpace(request.jefeProcesoId))
        {
            jefeProceso = await _userManager.FindByIdAsync(request.jefeProcesoId);
            
            if (jefeProceso is null)
            {
                return Result.Failure<ProcesoDto>(
                    Error.NotFound("Usuario", request.jefeProcesoId)
                );
            }
        }
        
        ProcesoModel newProceso = new()
        {
            Nombre = request.nombre,
            JefeDeProcesoId = jefeProceso?.Id,
            CreatedAt = DateTime.UtcNow
        };
        
        if (jefeProceso is not null)
        {
            newProceso.JefeDeProceso = jefeProceso;
            
            try
            {
                await _roleChangesService.PromoteToJefeProcesoAsync(jefeProceso.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<ProcesoDto>(
                    Error.Business("PromotionFailed", $"No se pudo promover al usuario: {ex.Message}")
                );
            }
        }
        
        try
        {
            _uow.Proceso.Add(newProceso);
            await _uow.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.Failure<ProcesoDto>(
                Error.Failure("DatabaseError", $"Error al guardar el proceso: {ex.Message}")
            );
        }

        var response = new ProcesoDto()
        {
            Id = newProceso.Id,
            Nombre = newProceso.Nombre,
            JefeProcesoId = newProceso.JefeDeProcesoId ?? "No Asignado",
            JefeProcesoNombre = newProceso.JefeDeProceso?.FullName ?? "No Asignado",
            Email = newProceso.JefeDeProceso?.Email ?? "N/A"
        };
      
        return Result.Success(response);
    }
}

