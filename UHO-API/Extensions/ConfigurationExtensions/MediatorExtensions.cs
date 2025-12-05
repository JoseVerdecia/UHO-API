

using UHO_API.Core.Interfaces;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Features.Authentication.Queries;
using UHO_API.Shared.Mediator;
using IMediator = UHO_API.Core.Interfaces.IMediator;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class MediatorExtensions
{
    public static void AddMediatorHandlersConfiguration(this IServiceCollection services)
    {
        //services.AddSingleton<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<RegisterRequest, AuthenticationResponse>, RegisterHandler>();
        services.AddScoped<IRequestHandler<LoginRequest, AuthenticationResponse>, LoginHandler>();
        services.AddScoped<IRequestHandler<RefreshTokenRequest, AuthenticationResponse>, RefreshTokenHandler>();
    }
}