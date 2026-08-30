namespace CodeWay.API.Common;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { Succeeded = true, Data = data, Message = message };

    public static ApiResponse<T> Failure(string message, List<string>? errors = null) =>
        new() { Succeeded = false, Message = message, Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string? message = null) =>
        new() { Succeeded = true, Message = message };

    public static new ApiResponse Failure(string message, List<string>? errors = null) =>
        new() { Succeeded = false, Message = message, Errors = errors };
}
