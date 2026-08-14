namespace InterviewAce.Application.DTOs.Interview;

public class InterviewSessionResponseDto
{
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = string.Empty;
}