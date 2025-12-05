using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Infraestructure.Services;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Authentication.Queries;


public record RegisterRequest(string Email, string Password,string FullName) : IRequest<AuthenticationResponse>;
public class RegisterHandler : IRequestHandler<RegisterRequest, AuthenticationResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService, IValidator<RegisterRequest> validator)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _validator = validator;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<AuthenticationResponse>(
                Error.Validation("Registro", errors)
            );
        }
        
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        
        if (existingUser is not null)
        {
            return Result.Failure<AuthenticationResponse>(
                Error.Conflict("Usuario", "Email", request.Email)
            );
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var identityResult = await _userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthenticationResponse>(
                Error.Validation("Registro", errors)
            );
        }
        
        await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
        
        // Generar tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Guardar refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthenticationResponse(
            user.Id,
            Roles.UsuarioNormal,
            user.FullName,
            user.Email!,
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15)
        ));
    }
}