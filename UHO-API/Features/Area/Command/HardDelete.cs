using Microsoft.AspNetCore.Identity;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record HardDeleteCommand(int Id):IRequest<bool>;
public class HardDeleteHandler : IRequestHandler<HardDeleteCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly ILogger<HardDeleteHandler> _logger;

    public HardDeleteHandler(
        IUnitOfWorks uow, 
        UserManager<ApplicationUser> userManager,
        IRoleChangesService roleChangesService,
        ILogger<HardDeleteHandler> logger)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _logger = logger;
    }
    
    public async Task<Result<bool>> Handle(HardDeleteCommand request, CancellationToken cancellationToken)
    {
        var area = await _uow.Area.GetById(request.Id);
        
        if (area is null)
        {
            return Result.Failure<bool>(Error.NotFound("Area", request.Id.ToString()));
        }
        
        
        if (!string.IsNullOrWhiteSpace(area.JefeAreaId))
        {
            try
            {
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(area.JefeAreaId);
                
                if (demotionResult.IsFailure)
                { 
                        _logger.LogWarning("No se pudo demover al jefe de área {JefeId} al eliminar área {AreaId}",
                        area.JefeAreaId, area.Id);
                }
            }
            catch (Exception ex)
            {
                    _logger.LogError(ex, "Error al demover jefe de área {JefeId} al eliminar área {AreaId}", 
                    area.JefeAreaId, area.Id);
            }
        }
        
        try
        {
            _uow.Area.Delete(area);
            await _uow.SaveChangesAsync();
            
            _logger.LogInformation("Área {AreaId} eliminada permanentemente", request.Id);
            
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente el área {AreaId}", request.Id);
            
            return Result.Failure<bool>(
                Error.Failure("DeleteFailed", $"Error al eliminar el área: {ex.Message}")
            );
        }
    }
}