namespace DrunkenBestManEFSM.Application.Results;

public class UseCaseResult<T>
{
    public bool Success { get; init; }

    public string MessageKey { get; init; } = string.Empty;

    public T? Data { get; init; }

    public static UseCaseResult<T> Ok(T data, string messageKey)
    {
        return new UseCaseResult<T>
        {
            Success = true,
            MessageKey = messageKey,
            Data = data
        };
    }

    public static UseCaseResult<T> Fail(string messageKey)
    {
        return new UseCaseResult<T>
        {
            Success = false,
            MessageKey = messageKey,
            Data = default
        };
    }
}
