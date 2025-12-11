using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Commands;


public record SoftDeleteProcesoCommand(int Id) : IRequest<bool>;
public class SoftDeleteProcesoHandler : IRequestHandler<SoftDeleteProcesoCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<SoftDeleteProcesoHandler> _logger;

    public SoftDeleteProcesoHandler(
        IUnitOfWorks uow, 
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<SoftDeleteProcesoHandler> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(SoftDeleteProcesoCommand request, CancellationToken cancellationToken)
    {
        var ProcesoToDelete = await _uow.Proceso.GetById(request.Id);
        
        if (ProcesoToDelete is null)
        {
            return Result.Failure<bool>(Error.NotFound("Proceso", request.Id.ToString()));
        }
        
        if (ProcesoToDelete.IsDeleted)
        {
            return Result.Failure<bool>(
                Error.Business("AlreadyDeleted", "El proceso ya está marcada como eliminado")
            );
        }

        
        if (!string.IsNullOrWhiteSpace(ProcesoToDelete.JefeDeProcesoId))
        {
            try
            {
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(ProcesoToDelete.JefeDeProcesoId);
                if (demotionResult.IsFailure)
                {
                    _logger.LogWarning("No se pudo demover al jefe de proceso {JefeId} al eliminar proceso {ProcesoId}", 
                        ProcesoToDelete.JefeDeProcesoId, ProcesoToDelete.Id);
                }
                
                
                ProcesoToDelete.JefeDeProcesoId = null;
                ProcesoToDelete.JefeDeProceso = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al demover jefe de proceso {JefeId} al eliminar proceso {ProcesoId}", 
                    ProcesoToDelete.JefeDeProcesoId, ProcesoToDelete.Id);
                
            }
        }
        
        try
        {
            await _uow.Proceso.SoftDelete(request.Id);
            await _uow.SaveChangesAsync();
            
            _logger.LogInformation("Proceso {ProcesoId} marcado como eliminado (soft delete)", request.Id);
            
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar como eliminado el proceso {ProcesoId}", request.Id);
            
            return Result.Failure<bool>(
                Error.Failure("SoftDeleteFailed", $"Error al marcar como eliminado el proceso: {ex.Message}")
            );
        }
    }
}