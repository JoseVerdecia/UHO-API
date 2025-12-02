using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;

public record GetAllJefeProcesos:IRequest<IEnumerable<ApplicationUser>>;


public class GetAllJefeProcesosQueryHandler : IRequestHandler<GetAllJefeProcesos,IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllJefeProcesosQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetAllJefeProcesos request, CancellationToken cancellationToken)
    {
        var allJefeProcesos = await _userManager.GetUsersInRoleAsync(Roles.JefeProceso);

        if (!allJefeProcesos.Any())
            return Error.NotFound("No hay usuarios en el sistema con el Rol: JefeProceso");

        return allJefeProcesos.ToList();
    }
}