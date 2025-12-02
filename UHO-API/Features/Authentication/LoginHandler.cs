using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.Services;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Authentication;

public record LoginRequest(string Email, string Password) : IRequest<AuthenticationResponse>;

// Features/Authentication/Login/LoginHandler.cs
public class LoginHandler : IRequestHandler<LoginRequest, AuthenticationResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public LoginHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthenticationResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Error.Validation("Email o contraseña invalidos.");
        }

        // Generar tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Guardar refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthenticationResponse(
            user.Id,
            roles.FirstOrDefault()!,
            user.FullName,
            user.Email!,
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15)
        );
    }
}