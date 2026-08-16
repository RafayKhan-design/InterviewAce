namespace InterviewAce.Application.DTOs.Interview;

public class AnswerEvaluationResponseDto
{
    public Guid Id { get; set; }

    public Guid InterviewAnswerId { get; set; }

    public int Score { get; set; }

    public List<string> Strengths { get; set; } = new();

    public List<string> Weaknesses { get; set; } = new();

    public string Feedback { get; set; } = string.Empty;

    public string IdealAnswer { get; set; } = string.Empty;

    public List<string> MissingTopics { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}