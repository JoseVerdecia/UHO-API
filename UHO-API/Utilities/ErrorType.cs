namespace UHO_API.Utilities;

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