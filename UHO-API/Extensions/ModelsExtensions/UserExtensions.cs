using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Features.Users.Commands;
using UHO_API.Features.Users.Queries;

namespace UHO_API.Extensions.ModelsExtensions;

public static class UserExtensions
{
    public static void AddUserQueriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GetUserQuery, ApplicationUser>, GetUserQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllUsersQuery, IEnumerable<ApplicationUser>>, GetAllUserQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllJefeAreasQuery, IEnumerable<ApplicationUser>>, GetAllJefeAreasQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllUsuariosNormalQuery, IEnumerable<ApplicationUser>>, GetAllUsuariosNormalQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllJefeProcesosQuery, IEnumerable<ApplicationUser>>, GetAllJefeProcesosQueryHandler>();
    }

    public static void AddUserCommandsConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<DegradeUserToUsuarioNormalCommand, ApplicationUser>, DegradeUserToUsuarioNormalHandler>();
    }
}