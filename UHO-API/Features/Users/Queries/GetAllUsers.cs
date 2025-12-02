using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users.Queries;

public record GetAllUsersQuery():IRequest<IEnumerable<ApplicationUser>>;


public class GetAllUserQueryHandler:IRequestHandler<GetAllUsersQuery, IEnumerable<ApplicationUser>>
{
   
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUserQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userManager.Users.ToListAsync(cancellationToken);

        if (allUsers is null)
            return Error.NotFound("No hay usuarios en el sistema");

        return allUsers;

    }
}