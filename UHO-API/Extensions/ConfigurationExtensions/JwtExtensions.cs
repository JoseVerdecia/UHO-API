
using UHO_API.Infraestructure.Services;
using UHO_API.Infraestructure.Settings;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class JwtExtensions
{
    public static void JwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtService, JwtService>();
    }
}