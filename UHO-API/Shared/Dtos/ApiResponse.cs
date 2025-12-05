

using UHO_API.Shared.Results;

namespace UHO_API.Shared.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public DateTime Timestamp { get; init; }
    public Error? Error { get; init; }
    public string? Message { get; init; }
        
    public static ApiResponse<T> CreateSuccess(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Timestamp = DateTime.UtcNow,
        Message = message
    };
        
    public static ApiResponse<T> CreateFailure(Error error) => new()
    {
        Success = false,
        Timestamp = DateTime.UtcNow,
        Error = error,
        Message = error.Message
    };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public DateTime Timestamp { get; init; }
    public Error? Error { get; init; }
    public string? Message { get; init; }

    public static ApiResponse CreateSuccess(string? message = null) => new()
    {
        Success = true,
        Timestamp = DateTime.UtcNow,
        Message = message
    };

    public static ApiResponse CreateFailure(Error error) => new()
    {
        Success = false,
        Timestamp = DateTime.UtcNow,
        Error = error,
        Message = error.Message
    };
}