namespace UHO_API.Utilities;

public sealed record Error(string Message, string Code)
{
    // Errores comunes predefinidos
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Null value was provided", "NullValue");
    public static Error Validation(string message) => new(message, "Validation");
    public static Error NotFound(string message = "Resource not found") => new(message, "NotFound");
    public static Error Conflict(string message) => new(message, "Conflict");
    public static Error Failure(string message) => new(message, "Failure"); // Errores generales de negocio
    public static Error Unauthorized(string message = "Unauthorized") => new(message, "Unauthorized");
    public static Error Forbidden(string message = "Forbidden") => new(message, "Forbidden");
}