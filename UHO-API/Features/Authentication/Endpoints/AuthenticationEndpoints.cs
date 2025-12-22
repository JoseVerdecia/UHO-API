
using UHO_API.Core.Extensions;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Features.Authentication.Queries;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Authentication.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("auth").WithTags("Autenticacion");

        authGroup.MapPost("/login", Login);
        authGroup.MapPost("/register", Register);
        authGroup.MapPost("/refresh-token", RefreshToken);
    }

    private static async Task<IResult> RefreshToken(RefreshTokenRequest refreshTokenRequest,IMediator mediator)
    {
        Result<AuthenticationResponse> result = await mediator.Send<RefreshTokenRequest, AuthenticationResponse>(refreshTokenRequest);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Register(RegisterRequest request, IMediator mediator)
    {
        Result<AuthenticationResponse> result = await mediator.Send<RegisterRequest, AuthenticationResponse>(request);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Login(LoginRequest request,IMediator mediator)
    {
        Result<AuthenticationResponse> result = await mediator.Send<LoginRequest, AuthenticationResponse>(request);
        return result.ToHttpResult();
    }
}