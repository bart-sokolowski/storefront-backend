namespace StorefrontApi.Common;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Note { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResult<T> Ok(T data, string? note = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Note = note
    };

    public static ApiResult<T> Fail(string note, IEnumerable<string>? errors = null) => new()
    {
        IsSuccess = false,
        Note = note,
        Errors = errors
    };
}

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public string? Note { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResult Ok(string? note = null) => new()
    {
        IsSuccess = true,
        Note = note
    };

    public static ApiResult Fail(string note, IEnumerable<string>? errors = null) => new()
    {
        IsSuccess = false,
        Note = note,
        Errors = errors
    };
}
