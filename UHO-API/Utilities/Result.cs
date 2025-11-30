namespace UHO_API.Utilities;
// Result.cs
// Domain/Result.cs
public readonly struct Result<T>
{
    public readonly T Value;
    public readonly Error Error;

    public bool IsSuccess => Error == Error.None;
    public bool IsFailure => !IsSuccess;

    private Result(T value, Error error)
    {
        Value = value;
        Error = error;
    }

    // Fábricas públicas 
    public static Result<T> Success(T value) => new(value, Error.None);
    public static Result<T> Failure(Error error) => new(default(T)!, error);

    // Conversiones implícitas 
    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);

    // --- MÉTODOS CORREGIDOS ---

    /// <summary>
    /// Transforma el valor de un Result exitoso usando una función.
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> func)
    {
        return IsSuccess 
            ? Result<TResult>.Success(func(Value))  
            : Result<TResult>.Failure(Error);       
    }

    /// <summary>
    /// Encadena otra operación que retorna un Result. Si el primero es un fallo, se cortocircuita.
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func)
    {
        return IsSuccess 
            ? func(Value) 
            : Result<TResult>.Failure(Error);      
    }
    
    // --- Los otros métodos no necesitan cambios ---
    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }
        return this;
    }

    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        return IsSuccess && predicate(Value) ? this : Failure(error);
    }
    
    // El método Match es clave para la conversión final
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}