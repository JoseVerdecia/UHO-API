using UHO_API.Features.Authentication;
using UHO_API.Interfaces;
using UHO_API.Utilities;

namespace UHO_API.Endpoints;

public static class MediatorExtensions
{
    public static void AddMediatorHandlersConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<RegisterRequest, AuthenticationResponse>, RegisterHandler>();
        services.AddScoped<IRequestHandler<LoginRequest, AuthenticationResponse>, LoginHandler>();
        services.AddScoped<IRequestHandler<RefreshTokenRequest, AuthenticationResponse>, RefreshTokenHandler>();
    }
}