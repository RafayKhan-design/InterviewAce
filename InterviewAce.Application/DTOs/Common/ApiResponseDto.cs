namespace InterviewAce.Application.DTOs.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }


    public ApiResponseDto(
        bool success,
        string message,
        T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}