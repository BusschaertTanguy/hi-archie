namespace Common.Application.Models;

public class Result
{
    private protected Result(string? error = null)
    {
        Error = error;
    }

    public string? Error { get; }

    public static Result Success()
    {
        return new();
    }

    public static Result Failure(string error)
    {
        return new(error);
    }
}

public sealed class Result<TData> : Result
{
    private Result(TData? data, string? error) : base(error)
    {
        Data = data;
    }

    public TData? Data { get; }

    public static Result<TData> Success(TData data)
    {
        return new(data, null);
    }

    public new static Result<TData> Failure(string error)
    {
        return new(default, error);
    }
}