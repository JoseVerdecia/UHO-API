using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Infraestructure.Data;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class IdentiyExtensions
{
    public static void AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}