namespace CarAuction.Api.Shared.Model;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Data = data, Success = true, Message = message };

    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
