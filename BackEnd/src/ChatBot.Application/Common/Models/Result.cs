namespace ChatBot.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    protected void SetErrors(List<string> errors)
    {
        Errors = errors;
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors)
    {
        var result = new Result(false);
        result.SetErrors(errors); 
        return result;
    }
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    protected Result(bool isSuccess, T? value = default, string? error = null)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static new Result<T> Failure(string error) => new(false, default, error);
    public static new Result<T> Failure(List<string> errors)
    {
        var result = new Result<T>(false);
        result.SetErrors(errors); 
        return result;
    }
}