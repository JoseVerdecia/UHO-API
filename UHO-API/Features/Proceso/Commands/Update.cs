using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Commands;

public record UpdateProcesoCommand(int Id, string Nombre, string? JefeProcesoId) : IRequest<ProcesoDto>;


public class UpdateProcesoHandler : IRequestHandler<UpdateProcesoCommand, ProcesoDto>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<UpdateProcesoHandler> _logger;

    public UpdateProcesoHandler(
        IUnitOfWorks uow,
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<UpdateProcesoHandler> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }

    public async Task<Result<ProcesoDto>> Handle(UpdateProcesoCommand request, CancellationToken cancellationToken)
    {

        ProcesoModel? ProcesoToUpdate = await _uow.Proceso.Get(a => a.Id == request.Id);

        if (ProcesoToUpdate is null)
        {
            return Result.Failure<ProcesoDto>(
                Error.NotFound("Proceso", request.Id.ToString())
            );
        }


        var existingProcesoWithSameName = await _uow.Proceso.Get(a => a.Nombre == request.Nombre && a.Id != request.Id);

        if (existingProcesoWithSameName is not null)
        {
            return Result.Failure<ProcesoDto>(
                Error.Conflict("Proceso", "Nombre", request.Nombre)
            );
        }


        ApplicationUser? newJefeProceso = null;

        if (!string.IsNullOrWhiteSpace(request.JefeProcesoId))
        {
            newJefeProceso = await _userManager.FindByIdAsync(request.JefeProcesoId);
            if (newJefeProceso is null)
            {
                return Result.Failure<ProcesoDto>(
                    Error.NotFound("Usuario", request.JefeProcesoId)
                );
            }


            var existingJefeProcesoAssignment =
                await _uow.Proceso.Get(a => a.JefeDeProcesoId == request.JefeProcesoId && a.Id != request.Id);

            if (existingJefeProcesoAssignment is not null)
            {
                return Result.Failure<ProcesoDto>(
                    Error.Business("AlreadyAssigned",
                        $"El usuario {newJefeProceso.FullName} ya es jefe de otra área ({existingJefeProcesoAssignment.Nombre})")
                );
            }
        }


        var currentJefeId = ProcesoToUpdate.JefeDeProcesoId;

        using var transaction = await _uow.BeginTransactionAsync();
        try
        {

            if (!string.IsNullOrWhiteSpace(currentJefeId) &&
                currentJefeId != request.JefeProcesoId)
            {
                // Demover al jefe actual a UsuarioNormal
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(currentJefeId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al usuario {UserId} a UsuarioNormal", currentJefeId);
                }
            }


            if (newJefeProceso is not null && currentJefeId != newJefeProceso.Id)
            {

                if (!await _userManager.IsInRoleAsync(newJefeProceso, Roles.UsuarioNormal))
                {
                    return Result.Failure<ProcesoDto>(
                        Error.Business("InvalidRole",
                            "Solo usuarios con rol 'UsuarioNormal' pueden ser asignados como Jefe de Proceso")
                    );
                }


                var promotionResult = await _roleChangesService.PromoteToJefeProcesoAsync(newJefeProceso.Id);

                if (promotionResult.IsFailure)
                {
                    return Result.Failure<ProcesoDto>(
                        Error.Failure("PromotionFailed",
                            "No se pudo promover al usuario a Jefe de Proceso")
                    );
                }
            }


            if (string.IsNullOrWhiteSpace(request.JefeProcesoId) && !string.IsNullOrWhiteSpace(currentJefeId))
            {

                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(currentJefeId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al usuario {UserId} a UsuarioNormal al eliminar asignación",
                        currentJefeId);
                }
            }


            ProcesoToUpdate.Nombre = request.Nombre;
            ProcesoToUpdate.JefeDeProcesoId =
                string.IsNullOrWhiteSpace(request.JefeProcesoId) ? null : request.JefeProcesoId;
            ProcesoToUpdate.JefeDeProceso = newJefeProceso;
            ProcesoToUpdate.UpdatedAt = DateTime.UtcNow;

            _uow.Proceso.Update(ProcesoToUpdate);
            await _uow.SaveChangesAsync();


            await transaction.CommitAsync();


            var jefeProcesoInfo = await GetJefeProcesoInfoAsync(ProcesoToUpdate.JefeDeProcesoId);

            var response = new ProcesoDto()
            {
                Id = ProcesoToUpdate.Id,
                Nombre = ProcesoToUpdate.Nombre,
                JefeProcesoId = ProcesoToUpdate.JefeDeProcesoId ?? "No Asignado",
                JefeProcesoNombre = jefeProcesoInfo.fullName,
                Email = jefeProcesoInfo.email ?? "N/A"
            };

            _logger.LogInformation("Área {ProcesoId} actualizada exitosamente", ProcesoToUpdate.Id);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex, "Error al actualizar el área {ProcesoId}", request.Id);

            return Result.Failure<ProcesoDto>(
                Error.Failure("UpdateFailed",
                    $"Ocurrió un error inesperado al actualizar el área: {ex.Message}")
            );
        }
    }
    
    private async Task<(string fullName, string email)> GetJefeProcesoInfoAsync(string? jefeProcesoId)
    {
        if (string.IsNullOrWhiteSpace(jefeProcesoId))
        {
            return ("No Asignado", "N/A");
        }
        
        var user = await _userManager.FindByIdAsync(jefeProcesoId);
        if (user is null)
        {
            return ("Usuario No Encontrado", "N/A");
        }
        
        return (user.FullName ?? "Nombre No Disponible", user.Email ?? "Email No Disponible");
    }
}

