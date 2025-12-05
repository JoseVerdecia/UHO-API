namespace UHO_API.Features.Authentication.Dtos;

public record AuthenticationResponse(
    string UserId,
    string Rol,
    string Nombre,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresOn
);

