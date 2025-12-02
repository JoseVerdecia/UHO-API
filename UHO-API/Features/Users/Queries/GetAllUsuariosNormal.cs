using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;

public record GetAllUsuariosNormalQuery:IRequest<IEnumerable<ApplicationUser>>;


public class GetAllUsuariosNormalQueryHandler : IRequestHandler<GetAllUsuariosNormalQuery,IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsuariosNormalQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetAllUsuariosNormalQuery request, CancellationToken cancellationToken)
    {
        var allUsuariosNormal = await _userManager.GetUsersInRoleAsync(Roles.UsuarioNormal);

        if (!allUsuariosNormal.Any())
            return Error.NotFound("No hay usuarios en el sistema con el Rol: UsuarioNormal");

        return allUsuariosNormal.ToList();
    }
}