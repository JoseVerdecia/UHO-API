using UHO_API.Extensions;
using UHO_API.Features.Authentication;
using UHO_API.Interfaces;
using UHO_API.Utilities;


namespace UHO_API.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("auth");

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