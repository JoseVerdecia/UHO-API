using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Commands;

public record HardDeleteProcesoCommand(int Id):IRequest<bool>;


public class HardDeleteProcesoHandler : IRequestHandler<HardDeleteProcesoCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<HardDeleteProcesoCommand> _logger;

    public HardDeleteProcesoHandler(
        IUnitOfWorks uow, 
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<HardDeleteProcesoCommand> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }
    
    public async Task<Result<bool>> Handle(HardDeleteProcesoCommand request, CancellationToken cancellationToken)
    {
        var Proceso = await _uow.Proceso.GetById(request.Id);
        
        if (Proceso is null)
        {
            return Result.Failure<bool>(Error.NotFound("Proceso", request.Id.ToString()));
        }
        
        
        if (!string.IsNullOrWhiteSpace(Proceso.JefeDeProcesoId))
        {
            try
            {
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(Proceso.JefeDeProcesoId);
                
                if (demotionResult.IsFailure)
                { 
                        _logger.LogWarning("No se pudo demover al jefe de proceso {JefeId} al eliminar proceso {ProcesoId}",
                        Proceso.JefeDeProcesoId, Proceso.Id);
                }
            }
            catch (Exception ex)
            {
                    _logger.LogError(ex, "Error al demover jefe de proceso {JefeId} al eliminar proceso {ProcesoId}", 
                    Proceso.JefeDeProcesoId, Proceso.Id);
            }
        }
        
        try
        {
            _uow.Proceso.Delete(Proceso);
            await _uow.SaveChangesAsync();
            
            _logger.LogInformation("proceso {ProcesoId} eliminada permanentemente", request.Id);
            
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente el proceso {ProcesoId}", request.Id);
            
            return Result.Failure<bool>(
                Error.Failure("DeleteFailed", $"Error al eliminar el proceso: {ex.Message}")
            );
        }
    }
}