using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UHO_API.Infraestructure.Settings;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class AuthJwtExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings? settings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings!.Issuer,
                ValidAudience = settings!.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings!.SecretKey))
            };
        });
    }
}