using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Users.Queries;
public record GetUserQuery(string Id) : IRequest<ApplicationUser>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetUserQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<ApplicationUser>> Handle(
        GetUserQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                return Result.Failure<ApplicationUser>(
                    Error.Validation("Id", "El ID del usuario es requerido")
                );
            }

            var user = await _userManager.FindByIdAsync(request.Id);
            
            if (user is null)
            {
                return Result.Failure<ApplicationUser>(
                    Error.NotFound("Usuario", request.Id)
                );
            }
            
            if (user.IsDeleted)
            {
                return Result.Failure<ApplicationUser>(
                    Error.Business("UserDeleted", "El usuario ha sido eliminado")
                );
            }

            _logger.LogInformation("Usuario {UserId} obtenido exitosamente", request.Id);
            
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", request.Id);
            return Result.Failure<ApplicationUser>(
                Error.Failure("QueryFailed", $"Error al obtener usuario: {ex.Message}")
            );
        }
    }
}