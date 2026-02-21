namespace Transcriber.Utils;

internal class ClientResult<T>
{
    private readonly T? _result;
    internal bool IsSuccessful { get; }
    internal string Message { get; }

    protected ClientResult(bool isSuccessful, T? result, string? message)
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

    internal static ClientResult<T> Success(T result) => new ClientResult<T>(true, result, string.Empty);
    internal static ClientResult<T> Failure(string message) => new ClientResult<T>(false, default, message);
}
