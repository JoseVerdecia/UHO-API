using Microsoft.AspNetCore.Identity;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record SoftDeleteAreaCommand(int Id) : IRequest<bool>;
public class SoftDeleteAreaHandler : IRequestHandler<SoftDeleteAreaCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<SoftDeleteAreaHandler> _logger;

    public SoftDeleteAreaHandler(
        IUnitOfWorks uow, 
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<SoftDeleteAreaHandler> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(SoftDeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var areaToDelete = await _uow.Area.GetById(request.Id);
        
        if (areaToDelete is null)
        {
            return Result.Failure<bool>(Error.NotFound("Area", request.Id.ToString()));
        }
        
        if (areaToDelete.IsDeleted)
        {
            return Result.Failure<bool>(
                Error.Business("AlreadyDeleted", "El área ya está marcada como eliminada")
            );
        }

        
        if (!string.IsNullOrWhiteSpace(areaToDelete.JefeAreaId))
        {
            try
            {
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(areaToDelete.JefeAreaId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al jefe de área {JefeId} al eliminar área {AreaId}", 
                        areaToDelete.JefeAreaId, areaToDelete.Id);
                }
                
                
                areaToDelete.JefeAreaId = null;
                areaToDelete.JefeArea = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al demover jefe de área {JefeId} al eliminar área {AreaId}", 
                    areaToDelete.JefeAreaId, areaToDelete.Id);
                
            }
        }
        
        try
        {
            await _uow.Area.SoftDelete(request.Id);
            await _uow.SaveChangesAsync();
            
            _logger.LogInformation("Área {AreaId} marcada como eliminada (soft delete)", request.Id);
            
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar como eliminada el área {AreaId}", request.Id);
            
            return Result.Failure<bool>(
                Error.Failure("SoftDeleteFailed", $"Error al marcar como eliminada el área: {ex.Message}")
            );
        }
    }
}