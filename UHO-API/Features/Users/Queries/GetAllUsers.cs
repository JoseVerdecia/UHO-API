using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;



public record GetAllUsersQuery : IRequest<IEnumerable<ApplicationUser>>;

public class GetAllUserQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllUserQueryHandler> _logger;

    public GetAllUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllUserQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(
        GetAllUsersQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var allUsers = await _userManager.Users
                .Where(u => !u.IsDeleted) // Solo usuarios activos
                .ToListAsync(cancellationToken);

            if (!allUsers.Any())
            {
                return Result.Success<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
            }

            _logger.LogInformation("Se obtuvieron {Count} usuarios", allUsers.Count);
            
            return Result.Success<IEnumerable<ApplicationUser>>(allUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los usuarios");
            return Result.Failure<IEnumerable<ApplicationUser>>(
                Error.Failure("QueryFailed", $"Error al obtener usuarios: {ex.Message}")
            );
        }
    }
}