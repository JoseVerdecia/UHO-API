namespace UHO_API.Infraestructure.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan AccessTokenExpiration { get; set; }
    public TimeSpan RefreshTokenExpiration { get; set; }
}