namespace Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public List<string> ValidationErrors { get; set; }

    private Result(bool isSuccess, T? value, string? error = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = new List<string>();
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> ValidationFailure(List<string> errors) => new(false, default) { ValidationErrors = errors };
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> ValidationErrors { get; set; }

    private Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ValidationErrors = new List<string>();
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result ValidationFailure(List<string> errors) => new(false) { ValidationErrors = errors };
} 