
    using UHO_API.Shared.Dtos;
    using UHO_API.Shared.Results;

    namespace UHO_API.Extensions;

    public static class ResultExtensions
    {
        
        private static readonly Dictionary<string, (int StatusCode, string Title)> ErrorMappings = new()
        {
            ["Validation"] = (StatusCodes.Status400BadRequest, "Error de validación"),
            ["NotFound"] = (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            ["Conflict"] = (StatusCodes.Status409Conflict, "Conflicto"),
            ["Unauthorized"] = (StatusCodes.Status401Unauthorized, "No autorizado"),
            ["Forbidden"] = (StatusCodes.Status403Forbidden, "Acceso denegado"),
            ["Business"] = (StatusCodes.Status422UnprocessableEntity, "Error de negocio"),
            ["Failure"] = (StatusCodes.Status400BadRequest, "Error en la solicitud")
        };
        
        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            return result.Match(
                onSuccess: value => Results.Ok(ApiResponse<T>.CreateSuccess(value)),
                onFailure: error => MapErrorToHttpResult(error)
            );
        }
        
        public static IResult ToHttpResult(this Result result)
        {
            return result.Match(
                onSuccess: () => Results.Ok(ApiResponse.CreateSuccess()),
                onFailure: error => MapErrorToHttpResult(error)
            );
        }
        
        
        private static IResult MapErrorToHttpResult(Error error)
        {
            if (error == Error.None)
                return Results.Ok();
                
            return error.Type switch
            {
                ErrorType.Validation => CreateValidationProblem(error),
                ErrorType.NotFound => Results.NotFound(ApiResponse.CreateFailure(error)),
                ErrorType.Conflict => Results.Conflict(ApiResponse.CreateFailure(error)),
                ErrorType.Unauthorized => Results.Unauthorized(),
                ErrorType.Forbidden => Results.Forbid(),
                ErrorType.Business => Results.UnprocessableEntity(ApiResponse.CreateFailure(error)),
                _ => Results.Problem(
                    detail: error.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error interno del servidor",
                    type: $"https://httpstatuses.com/{StatusCodes.Status500InternalServerError}"
                )
            };
        }
        
        private static IResult CreateValidationProblem(Error error)
        {
            return Results.Problem(
                detail: error.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Error de validación",
                type: $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}",
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = error.Code,
                    ["errors"] = new[] { new { Field = error.Code.Replace("Validation.", ""), error.Message } }
                }
            );
        }
    }



   

   