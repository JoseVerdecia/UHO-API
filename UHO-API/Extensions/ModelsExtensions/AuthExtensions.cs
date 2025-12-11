using UHO_API.Core.Interfaces;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Features.Authentication.Queries;

namespace UHO_API.Extensions.ModelsExtensions;

public static class AuthExtensions
{
    public static void AddAuthCommandsConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<RegisterRequest, AuthenticationResponse>, RegisterHandler>();
        services.AddScoped<IRequestHandler<LoginRequest, AuthenticationResponse>, LoginHandler>();
        services.AddScoped<IRequestHandler<RefreshTokenRequest, AuthenticationResponse>, RefreshTokenHandler>();
    }
}