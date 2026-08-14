namespace InterviewAce.Application.DTOs.Interview;

public class InterviewAnswerResponseDto
{
    public Guid Id { get; set; }

    public Guid InterviewSessionId { get; set; }

    public Guid InterviewQuestionId { get; set; }

    public string Answer { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
}