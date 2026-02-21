namespace Transcriber.Utils;

internal class Result<T>
{
    private readonly T? _result;
    internal bool IsSuccessful { get; }
    internal string Message { get; }

    protected Result(bool isSuccessful, T? result, string? message)
    {
        IsSuccessful = isSuccessful;
        _result = result;
        Message = message ?? string.Empty;
    }

    internal T GetResult()
    {
        if (!IsSuccessful)
        {
            throw new InvalidOperationException("Result was not successful, there is nothing to return.");
        }

        return _result!;
    }

    internal static Result<T> Success(T result) => new Result<T>(true, result, string.Empty);
    internal static Result<T> Failure(string message) => new Result<T>(false, default, message);
}
