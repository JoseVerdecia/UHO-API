using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UHO_API.Data.Settings;
using UHO_API.Infraestructure.Services;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class JwtExtensions
{
    public static void JwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtService, JwtService>();
    }
}