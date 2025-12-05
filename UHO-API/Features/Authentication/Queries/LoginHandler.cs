using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Infraestructure.Services;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Authentication.Queries;

public record LoginRequest(string Email, string Password) : IRequest<AuthenticationResponse>;
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
        
        // Verificar si el usuario existe y no está eliminado
        if (user is null || user.IsDeleted || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result.Failure<AuthenticationResponse>(
                Error.Validation("Credenciales", "Email o contraseña inválidos")
            );
        }

        // Generar tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Guardar refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthenticationResponse(
            user.Id,
            roles.FirstOrDefault()!,
            user.FullName,
            user.Email!,
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15)
        ));
    }
}