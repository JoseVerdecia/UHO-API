using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Users.Queries;
public record GetAllUsuariosNormalQuery : IRequest<IEnumerable<ApplicationUser>>;

public class GetAllUsuariosNormalQueryHandler : IRequestHandler<GetAllUsuariosNormalQuery, IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllUsuariosNormalQueryHandler> _logger;

    public GetAllUsuariosNormalQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllUsuariosNormalQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(
        GetAllUsuariosNormalQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var usuariosNormal = await _userManager.GetUsersInRoleAsync(Roles.UsuarioNormal);
            
            if (!usuariosNormal.Any())
            {
                return Result.Success<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
            }

            // Filtrar usuarios no eliminados
            var activeUsuariosNormal = usuariosNormal.Where(u => !u.IsDeleted).ToList();
            
            _logger.LogInformation("Se obtuvieron {Count} usuarios normales", activeUsuariosNormal.Count);
            
            return Result.Success<IEnumerable<ApplicationUser>>(activeUsuariosNormal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios normales");
            return Result.Failure<IEnumerable<ApplicationUser>>(
                Error.Failure("QueryFailed", $"Error al obtener usuarios normales: {ex.Message}")
            );
        }
    }
}