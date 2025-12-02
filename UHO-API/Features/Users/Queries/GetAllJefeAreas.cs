using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;

public record GetAllJefeAreasQuery:IRequest<IEnumerable<ApplicationUser>>;

public class GetAllJefeAreasQueryHandler:IRequestHandler<GetAllJefeAreasQuery, IEnumerable<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllJefeAreasQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetAllJefeAreasQuery request, CancellationToken cancellationToken)
    {
        var allJefeArea = await _userManager.GetUsersInRoleAsync(Roles.JefeArea);

        if (!allJefeArea.Any())
            return Error.NotFound("No hay usuarios en el sistema con el Rol: JefeArea");

        return allJefeArea.ToList();

    }
}