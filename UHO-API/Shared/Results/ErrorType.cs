namespace UHO_API.Shared.Results;

public enum ErrorType
{
    None,
    Validation,
    NullValue,
    NotFound,
    Conflict,
    Failure,
    Unauthorized,
    Forbidden,
    Business,
    Server
}