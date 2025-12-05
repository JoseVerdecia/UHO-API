using Microsoft.AspNetCore.Identity;
using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record UpdateAreaCommand(int Id, string Nombre, string? JefeAreaId) : IRequest<AreaResponse>;


 public class UpdateAreaHandler : IRequestHandler<UpdateAreaCommand, AreaResponse>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<UpdateAreaHandler> _logger;

    public UpdateAreaHandler(
        IUnitOfWorks uow, 
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<UpdateAreaHandler> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }

    public async Task<Result<AreaResponse>> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
      
        AreaModel? areaToUpdate = await _uow.Area.Get(a=>a.Id == request.Id);
        
        if (areaToUpdate is null)
        {
            return Result.Failure<AreaResponse>(
                Error.NotFound("Area", request.Id.ToString())
            );
        }
        
        
        var existingAreaWithSameName = await _uow.Area.Get(
            a => a.Nombre == request.Nombre && a.Id != request.Id);
        
        if (existingAreaWithSameName is not null)
        {
            return Result.Failure<AreaResponse>(
                Error.Conflict("Area", "Nombre", request.Nombre)
            );
        }
        
        
        ApplicationUser? newJefeArea = null;
        
        if (!string.IsNullOrWhiteSpace(request.JefeAreaId))
        {
            newJefeArea = await _userManager.FindByIdAsync(request.JefeAreaId);
            if (newJefeArea is null)
            {
                return Result.Failure<AreaResponse>(
                    Error.NotFound("Usuario", request.JefeAreaId)
                );
            }
            
           
            var existingJefeAreaAssignment = await _uow.Area.Get(
                a => a.JefeAreaId == request.JefeAreaId && a.Id != request.Id);
            
            if (existingJefeAreaAssignment is not null)
            {
                return Result.Failure<AreaResponse>(
                    Error.Business("AlreadyAssigned", 
                        $"El usuario {newJefeArea.FullName} ya es jefe de otra área ({existingJefeAreaAssignment.Nombre})")
                );
            }
        }
        
     
        var currentJefeId = areaToUpdate.JefeAreaId;
        
        using var transaction = await _uow.BeginTransactionAsync();
        try
        {
    
            if (!string.IsNullOrWhiteSpace(currentJefeId) && 
                currentJefeId != request.JefeAreaId)
            {
                // Demover al jefe actual a UsuarioNormal
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(currentJefeId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al usuario {UserId} a UsuarioNormal", currentJefeId);
                }
            }
            
          
            if (newJefeArea is not null && currentJefeId != newJefeArea.Id)
            {
                
                if (!await _userManager.IsInRoleAsync(newJefeArea, Roles.UsuarioNormal))
                {
                    return Result.Failure<AreaResponse>(
                        Error.Business("InvalidRole", 
                            "Solo usuarios con rol 'UsuarioNormal' pueden ser asignados como Jefe de Área")
                    );
                }
                
               
                var promotionResult = await _roleChangesService.PromoteToJefeAreaAsync(newJefeArea.Id);
                
                if (promotionResult.IsFailure)
                {
                    return Result.Failure<AreaResponse>(
                        Error.Failure("PromotionFailed", 
                            "No se pudo promover al usuario a Jefe de Área")
                    );
                }
            }
            
            
            if (string.IsNullOrWhiteSpace(request.JefeAreaId) && !string.IsNullOrWhiteSpace(currentJefeId))
            {
                
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(currentJefeId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al usuario {UserId} a UsuarioNormal al eliminar asignación", currentJefeId);
                }
            }
            
          
            areaToUpdate.Nombre = request.Nombre;
            areaToUpdate.JefeAreaId = string.IsNullOrWhiteSpace(request.JefeAreaId) ? null : request.JefeAreaId;
            areaToUpdate.JefeArea = newJefeArea; 
            areaToUpdate.UpdatedAt = DateTime.UtcNow;
            
            _uow.Area.Update(areaToUpdate);
            await _uow.SaveChangesAsync();
            
            
            await transaction.CommitAsync();
            
           
            var jefeAreaInfo = await GetJefeAreaInfoAsync(areaToUpdate.JefeAreaId);
            
            var response = new AreaResponse(
                areaToUpdate.Id,
                areaToUpdate.Nombre,
                areaToUpdate.JefeAreaId ?? "No Asignado",
                jefeAreaInfo.fullName,
                jefeAreaInfo.email
            );
            
            _logger.LogInformation("Área {AreaId} actualizada exitosamente", areaToUpdate.Id);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            
            _logger.LogError(ex, "Error al actualizar el área {AreaId}", request.Id);
            
            return Result.Failure<AreaResponse>(
                Error.Failure("UpdateFailed", 
                    $"Ocurrió un error inesperado al actualizar el área: {ex.Message}")
            );
        }
    }
    
    private async Task<(string fullName, string email)> GetJefeAreaInfoAsync(string? jefeAreaId)
    {
        if (string.IsNullOrWhiteSpace(jefeAreaId))
        {
            return ("No Asignado", "N/A");
        }
        
        var user = await _userManager.FindByIdAsync(jefeAreaId);
        if (user is null)
        {
            return ("Usuario No Encontrado", "N/A");
        }
        
        return (user.FullName ?? "Nombre No Disponible", user.Email ?? "Email No Disponible");
    }
}
