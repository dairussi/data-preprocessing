public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) =>
        new Result<T>(true, value, string.Empty);

    public static Result<T> Failure(string error) =>
        new Result<T>(false, default!, error);

    public TResult Match<TResult>(
        Func<T, TResult> success,
        Func<string, TResult> failure) =>
        IsSuccess ? success(Value) : failure(Error);
}

public static class Result
{
    public static Result<T> Success<T>(T value) =>
        Result<T>.Success(value);

    public static Result<T> Failure<T>(string error) =>
        Result<T>.Failure(error);
}
