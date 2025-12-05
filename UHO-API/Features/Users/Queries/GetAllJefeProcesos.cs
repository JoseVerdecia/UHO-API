using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;
public record GetAllJefeProcesosQuery : IRequest<IEnumerable<ApplicationUser>>;

public class GetAllJefeProcesosQueryHandler : IRequestHandler<GetAllJefeProcesosQuery, IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllJefeProcesosQueryHandler> _logger;

    public GetAllJefeProcesosQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllJefeProcesosQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(
        GetAllJefeProcesosQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var jefesProceso = await _userManager.GetUsersInRoleAsync(Roles.JefeProceso);
            
            if (!jefesProceso.Any())
            {
                return Result.Success<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
            }

            // Filtrar usuarios no eliminados
            var activeJefesProceso = jefesProceso.Where(u => !u.IsDeleted).ToList();
            
            _logger.LogInformation("Se obtuvieron {Count} jefes de proceso", activeJefesProceso.Count);
            
            return Result.Success<IEnumerable<ApplicationUser>>(activeJefesProceso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener jefes de proceso");
            return Result.Failure<IEnumerable<ApplicationUser>>(
                Error.Failure("QueryFailed", $"Error al obtener jefes de proceso: {ex.Message}")
            );
        }
    }
}