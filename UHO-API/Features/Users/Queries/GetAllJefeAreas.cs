using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;

public record GetAllJefeAreasQuery:IRequest<IEnumerable<ApplicationUser>>;

public class GetAllJefeAreasQueryHandler : IRequestHandler<GetAllJefeAreasQuery, IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllJefeAreasQueryHandler> _logger;

    public GetAllJefeAreasQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllJefeAreasQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(
        GetAllJefeAreasQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var jefesArea = await _userManager.GetUsersInRoleAsync(Roles.JefeArea);
            
            if (!jefesArea.Any())
            {
                return Result.Success<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
                // O puedes devolver un warning si prefieres:
                // return Result.Failure<IEnumerable<ApplicationUser>>(
                //     Error.Business("NoJefeAreas", "No hay jefes de área registrados")
                // );
            }

            // Filtrar usuarios no eliminados
            var activeJefesArea = jefesArea.Where(u => !u.IsDeleted).ToList();
            
            _logger.LogInformation("Se obtuvieron {Count} jefes de área", activeJefesArea.Count);
            
            return Result.Success<IEnumerable<ApplicationUser>>(activeJefesArea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener jefes de área");
            return Result.Failure<IEnumerable<ApplicationUser>>(
                Error.Failure("QueryFailed", $"Error al obtener jefes de área: {ex.Message}")
            );
        }
    }
}