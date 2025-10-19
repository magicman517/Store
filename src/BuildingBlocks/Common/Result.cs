namespace Common;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public int StatusCode { get; init; }

    public T Value { get; init; }

    public string Error { get; init; }

    private Result(T value, int statusCode, bool isSuccess, string error)
    {
        Value = value;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Ok(T value, int statusCode = 200) =>
        new(value, statusCode, true, string.Empty);

    public static Result<T> Fail(string error, int statusCode) =>
        new(default!, statusCode, false, error);
}