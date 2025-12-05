using UHO_API.Utilities;

namespace UHO_API.Extensions;

public static class ErrorExtensions
{
    public static IResult ToHttpResult(this Error error)
    {
        return Result.Failure<object>(error).ToHttpResult();
    }
}