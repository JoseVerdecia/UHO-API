
using UHO_API.Extensions;
using UHO_API.Shared.Results;

namespace UHO_API.Core.Extensions;

public static class ErrorExtensions
{
    public static IResult ToHttpResult(this Error error)
    {
        return Result.Failure<object>(error).ToHttpResult();
    }
}