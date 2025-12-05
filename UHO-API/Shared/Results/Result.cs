namespace UHO_API.Shared.Results;


public abstract class Result
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; protected init; } = Error.None;
    
    protected Result() { }
    
    // Métodos estáticos para éxito
    public static Result Success() => new SuccessResult();
    public static Result<T> Success<T>(T value) => new (value, true, Error.None);
    
    // Métodos estáticos para fallo
    public static Result Failure(Error error) => new FailureResult(error);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
    
    
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
    
    private sealed class SuccessResult : Result
    {
        public SuccessResult() => IsSuccess = true;
    }
    
    private sealed class FailureResult : Result
    {
        public FailureResult(Error error)
        {
            IsSuccess = false;
            Error = error;
        }
    }
    
}


public sealed class Result<T> : Result
{
    private readonly T? _value;
    
    public T Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access Value of a failed result");
    
    internal Result(T? value, bool isSuccess, Error error)
    {
        _value = value;
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return IsSuccess 
            ? Result.Success(mapper(_value!))
            : Result.Failure<TResult>(Error);
    }
    
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        return IsSuccess ? binder(_value!) : Result.Failure<TResult>(Error);
    }
    
    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess) action(_value!);
        return this;
    }
    
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        if (IsFailure) return this;
        return predicate(_value!) ? this : Result.Failure<T>(error);
    }
    
    public Result<T> Ensure(Func<T, bool> predicate, Func<T, Error> errorFactory)
    {
        if (IsFailure) return this;
        return predicate(_value!) ? this : Result.Failure<T>(errorFactory(_value!));
    }
    
    // Para operaciones que pueden fallar
    public Result<TResult> Try<TResult>(Func<T, TResult> operation, Func<Exception, Error> errorMapper)
    {
        if (IsFailure) return Result.Failure<TResult>(Error);
        
        try
        {
            return Result.Success(operation(_value!));
        }
        catch (Exception ex)
        {
            return Result.Failure<TResult>(errorMapper(ex));
        }
    }
    
    // Combinar resultados
    public static Result<T> Combine(params Result[] results)
    {
        var failedResult = results.FirstOrDefault(r => r.IsFailure);
        return failedResult != null 
            ? Result.Failure<T>(failedResult.Error)
            : Result.Success(default(T)!);
    }
    public IResult Match(
        Func<T, IResult> onSuccess,
        Func<Error, IResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
    
}