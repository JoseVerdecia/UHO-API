

namespace UHO_API.Shared.Results;

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    
    // Factory methods con códigos específicos
    public static Error Validation(string code, string message) 
        => new($"Validation.{code}", message, ErrorType.Validation);
    
    public static Error NotFound(string entity, string id) 
        => new($"{entity}.NotFound", $"{entity} con ID '{id}' no encontrado", ErrorType.NotFound);
    
    public static Error Conflict(string entity, string field, string value) 
        => new($"{entity}.Conflict", $"{entity} con {field} '{value}' ya existe", ErrorType.Conflict);
    
    public static Error Business(string code, string message) 
        => new($"Business.{code}", message, ErrorType.Business);
    
    public static Error Unauthorized(string message = "No autorizado") 
        => new("Auth.Unauthorized", message, ErrorType.Unauthorized);
    
    public static Error Forbidden(string message = "Acceso denegado") 
        => new("Auth.Forbidden", message, ErrorType.Forbidden);
    
    public static Error Failure(string code, string message) 
        => new($"Failure.{code}", message, ErrorType.Failure);
    
    public static Error Server(string code, string message) 
        => new($"Server.{code}", message, ErrorType.Server);
    
    public static Error NullValue(string code, string message) 
        => new($"NullValue.{code}", message, ErrorType.NullValue);
    
    // Para múltiples errores de validación
    public static List<Error> CreateValidationErrors(
        List<(string Field, string Message)> errors)
    {
        return errors.Select(e => 
            Validation(e.Field, e.Message)).ToList();
    }
}