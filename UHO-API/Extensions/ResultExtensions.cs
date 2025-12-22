using Microsoft.AspNetCore.Mvc;
using UHO_API.Shared.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Extensions;

public static class ResultExtensions
{
    private static readonly Dictionary<ErrorType, (int StatusCode, string Title)> ErrorMappings = new()
    {
        [ErrorType.Validation] = (StatusCodes.Status400BadRequest, "Error de validación"),
        [ErrorType.NotFound] = (StatusCodes.Status404NotFound, "Recurso no encontrado"),
        [ErrorType.Conflict] = (StatusCodes.Status409Conflict, "Conflicto"),
        [ErrorType.Unauthorized] = (StatusCodes.Status401Unauthorized, "No autorizado"),
        [ErrorType.Forbidden] = (StatusCodes.Status403Forbidden, "Acceso denegado"),
        [ErrorType.Business] = (StatusCodes.Status422UnprocessableEntity, "Error de negocio"),
        [ErrorType.Failure] = (StatusCodes.Status400BadRequest, "Error en la solicitud"),
        [ErrorType.NullValue] = (StatusCodes.Status400BadRequest, "Valor nulo"),
        [ErrorType.Server] = (StatusCodes.Status500InternalServerError, "Error interno del servidor"),
        [ErrorType.None] = (StatusCodes.Status200OK, "OK")
    };
    
    // Para Result<T> con múltiples errores
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        return result.Match(
            onSuccess: value => Results.Ok(ApiResponse<T>.CreateSuccess(value)),
            onFailure: errors => MapErrorsToHttpResult(errors)
        );
    }
    
    // Para Result con múltiples errores
    public static IResult ToHttpResult(this Result result)
    {
        return result.Match(
            onSuccess: () => Results.Ok(ApiResponse.CreateSuccess()),
            onFailure: errors => MapErrorsToHttpResult(errors)
        );
    }
    
    // Para compatibilidad con un solo error (si alguien pasa Error directamente)
    public static IResult ToHttpResult(this Error error)
    {
        if (error == Error.None)
            return Results.Ok();
            
        return MapErrorsToHttpResult([error]);
    }
    
    // Para compatibilidad con lista de errores
    public static IResult ToHttpResult(this IReadOnlyList<Error> errors)
    {
        return MapErrorsToHttpResult(errors);
    }
    
    private static IResult MapErrorsToHttpResult(IReadOnlyList<Error> errors)
    {
        if (!errors.Any())
            return Results.Ok();
        
        // Determinar el tipo de error principal (primero no-None)
        var firstError = errors.FirstOrDefault(e => e.Type != ErrorType.None);
        
        if (firstError == null)
            return Results.Ok();
        
        // Si hay errores de validación, devolver ValidationProblemDetails
        if (errors.Any(e => e.Type == ErrorType.Validation))
        {
            return CreateValidationProblemForErrors(errors);
        }
        
        // Para otros tipos, usar el primer error para determinar el código de estado
        return CreateProblemForErrors(errors, firstError);
    }
    
    private static IResult CreateValidationProblemForErrors(IReadOnlyList<Error> errors)
    {
        var validationErrors = new Dictionary<string, string[]>();
        var otherErrors = new List<Error>();
        
        // Separar errores de validación de otros tipos
        foreach (var error in errors)
        {
            if (error.Type == ErrorType.Validation)
            {
                // Extraer el nombre del campo del código
                var fieldName = error.Code.Replace("Validation.", "");
                
                if (validationErrors.ContainsKey(fieldName))
                {
                    validationErrors[fieldName] = validationErrors[fieldName]
                        .Concat(new[] { error.Message })
                        .ToArray();
                }
                else
                {
                    validationErrors[fieldName] = new[] { error.Message };
                }
            }
            else
            {
                otherErrors.Add(error);
            }
        }
        
        var problemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Error de validación",
            Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}"
        };
        
        // Agregar otros errores como extensión
        if (otherErrors.Any())
        {
            problemDetails.Extensions["otherErrors"] = otherErrors
                .Select(e => new { e.Code, e.Message, e.Type })
                .ToList();
        }
        
        return Results.Problem(problemDetails);
    }
    
    private static IResult CreateProblemForErrors(IReadOnlyList<Error> errors, Error firstError)
    {
        // OPCIÓN 1: Usar deconstrucción de tupla
        var (statusCode, title) = ErrorMappings.GetValueOrDefault(
            firstError.Type, 
            (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        );
        
        var problemDetails = new ProblemDetails
        {
            Detail = firstError.Message,
            Status = statusCode,  // Usar variable desconstruída
            Title = title,        // Usar variable desconstruída
            Type = $"https://httpstatuses.com/{statusCode}"
        };
        
        // Agregar todos los errores como extensión
        problemDetails.Extensions["errors"] = errors
            .Select(e => new { e.Code, e.Message, e.Type })
            .ToList();
        
        // Para errores específicos, devolver resultados específicos
        return firstError.Type switch
        {
            ErrorType.NotFound => Results.NotFound(problemDetails),
            ErrorType.Conflict => Results.Conflict(problemDetails),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.Business => Results.UnprocessableEntity(problemDetails),
            _ => Results.Problem(problemDetails)
        };
    }
    
    // OPCIÓN ALTERNATIVA: Si prefieres no usar deconstrucción, puedes hacer esto:
    private static IResult CreateProblemForErrorsAlt(IReadOnlyList<Error> errors, Error firstError)
    {
        var mapping = ErrorMappings.GetValueOrDefault(
            firstError.Type, 
            (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        );
        
        // Acceder a los elementos de la tupla por posición
        var statusCode = mapping.Item1;
        var title = mapping.Item2;
        
        var problemDetails = new ProblemDetails
        {
            Detail = firstError.Message,
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
        
        // Agregar todos los errores como extensión
        problemDetails.Extensions["errors"] = errors
            .Select(e => new { e.Code, e.Message, e.Type })
            .ToList();
        
        // Para errores específicos, devolver resultados específicos
        return firstError.Type switch
        {
            ErrorType.NotFound => Results.NotFound(problemDetails),
            ErrorType.Conflict => Results.Conflict(problemDetails),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.Business => Results.UnprocessableEntity(problemDetails),
            _ => Results.Problem(problemDetails)
        };
    }
    
    // Resto de los métodos se mantienen igual...
  
}