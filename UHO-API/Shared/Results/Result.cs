namespace UHO_API.Shared.Results;



public abstract class Result
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; protected init; } = Array.Empty<Error>();
    
    // Propiedad conveniente para compatibilidad (primer error)
    public Error FirstError => Errors.FirstOrDefault() ?? Error.None;
    
    protected Result() { }
    
    // Métodos estáticos para éxito
    public static Result Success() => new SuccessResult();
    public static Result<T> Success<T>(T value) => new (value, true, Array.Empty<Error>());
    
    // Métodos estáticos para fallo (mantener compatibilidad con un solo error)
    public static Result Failure(Error error) => new FailureResult([error]);
    public static Result<T> Failure<T>(Error error) => new(default!, false, [error]);
    
    // Métodos para múltiples errores
    public static Result Failure(IEnumerable<Error> errors) => new FailureResult(errors.ToList());
    public static Result<T> Failure<T>(IEnumerable<Error> errors) => new(default!, false, errors.ToList());
    public static Result Failure(params Error[] errors) => new FailureResult(errors);
    public static Result<T> Failure<T>(params Error[] errors) => new(default!, false, errors);
    
    
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Errors);
    }
    
    private sealed class SuccessResult : Result
    {
        public SuccessResult() => IsSuccess = true;
    }
    
    private sealed class FailureResult : Result
    {
        public FailureResult(IReadOnlyList<Error> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }
    }
}

public sealed class Result<T> : Result
{
    private readonly T? _value;
    
    public T Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access Value of a failed result");
    
    internal Result(T? value, bool isSuccess, IReadOnlyList<Error> errors)
    {
        _value = value;
        IsSuccess = isSuccess;
        Errors = errors;
    }
    
    // Constructor de conveniencia para un solo error
    internal Result(T? value, bool isSuccess, Error error)
        : this(value, isSuccess, [error])
    {
    }
    public static new Result<T> Success(T value) => new(value, true, Array.Empty<Error>());
    public static new Result<T> Failure(Error error) => new(default!, false, [error]);
    public static new Result<T> Failure(IEnumerable<Error> errors) => new(default!, false, errors.ToList());
    public static new Result<T> Failure(params Error[] errors) => new(default!, false, errors);
    
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return IsSuccess 
            ? Result.Success(mapper(_value!))
            : Result.Failure<TResult>(Errors);
    }
    
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        return IsSuccess ? binder(_value!) : Result.Failure<TResult>(Errors);
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
        if (IsFailure) return Result.Failure<TResult>(Errors);
        
        try
        {
            return Result.Success(operation(_value!));
        }
        catch (Exception ex)
        {
            return Result.Failure<TResult>(errorMapper(ex));
        }
    }
    
    // Combinar resultados con múltiples errores
    public static Result<T> Combine(params Result[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .SelectMany(r => r.Errors)
            .ToList();
        
        return errors.Any() 
            ? Result.Failure<T>(errors)
            : Result.Success(default(T)!);
    }
    
    // Método para combinar resultados de diferentes tipos
    public static Result<T> Combine<T1, T2>(Result<T1> result1, Result<T2> result2)
    {
        var errors = new List<Error>();
        
        if (result1.IsFailure) errors.AddRange(result1.Errors);
        if (result2.IsFailure) errors.AddRange(result2.Errors);
        
        return errors.Any()
            ? Result.Failure<T>(errors)
            : Result.Success(default(T)!);
    }
    
    // Método Match específico para Result<T>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(_value!) : onFailure(Errors);
    }
}