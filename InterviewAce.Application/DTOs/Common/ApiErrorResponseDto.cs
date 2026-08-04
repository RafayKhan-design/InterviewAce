namespace InterviewAce.Application.DTOs.Common;

public class ApiErrorResponseDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public object? Errors { get; set; }
}