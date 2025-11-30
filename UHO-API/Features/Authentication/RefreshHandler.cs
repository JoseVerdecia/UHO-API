using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UHO_API.Data.Settings;
using UHO_API.Infraestructure.Services;
using UHO_API.Interfaces;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Authentication;

public record RefreshTokenRequest(string AccessToken, string RefreshToken) : IRequest<AuthenticationResponse>;


public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, AuthenticationResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
        {
            return Error.Failure("Invalid access token.");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Error.Failure("Invalid token.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return Error.Failure("Invalid refresh token.");
        }

        // Generar nuevos tokens
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Actualizar refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthenticationResponse(
            user.Id,
            roles.FirstOrDefault()!,
            user.FullName,
            user.Email!,
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(15)
        );
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Importante: no validar expiración aquí
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }
}