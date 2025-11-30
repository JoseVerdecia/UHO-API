using UHO_API.Utilities;

namespace UHO_API.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Convierte un Result<T> del dominio a un IResult de ASP.NET Core usando el patrón Match.
    /// Este es el único lugar donde se mapean los errores de dominio a códigos de estado HTTP.
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        return result.Match<IResult>(
            onSuccess: value => Results.Ok(value), 
            onFailure: error => MapErrorToHttpResult(error) // Mapea el error a un resultado HTTP
        );
    }

    private static IResult MapErrorToHttpResult(Error error)
    {
        int statusCode = error.Code switch
        {
            "Validation" => StatusCodes.Status400BadRequest,
            "NotFound" => StatusCodes.Status404NotFound,
            "Conflict" => StatusCodes.Status409Conflict,
            "Unauthorized" => StatusCodes.Status401Unauthorized,
            "Failure" => StatusCodes.Status400BadRequest, 
            _ => StatusCodes.Status500InternalServerError 
        };
        
        return Results.Problem(
            detail: error.Message,
            statusCode: statusCode,
            title: "A ocurrido un error"
        );
    }
}