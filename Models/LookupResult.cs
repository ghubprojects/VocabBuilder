namespace VocabBuilder.Models;

public class LookupResult<T>
{
    public bool Success { get; }
    public string Error { get; }
    public T? Data { get; }

    private LookupResult(bool success, T? data, string error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public static LookupResult<T> Ok(T data) => new(true, data, string.Empty);
    public static LookupResult<T> Fail(string error) => new(false, default, error);
}