namespace InterviewAce.Domain.Entities;

public class AnswerEvaluation
{
    public Guid Id { get; set; }

    public Guid InterviewAnswerId { get; set; }

    public InterviewAnswer InterviewAnswer { get; set; } = null!;

    public int Score { get; set; }

    public string Strengths { get; set; } = "[]";

    public string Weaknesses { get; set; } = "[]";

    public string Feedback { get; set; } = string.Empty;

    public string IdealAnswer { get; set; } = string.Empty;

    public string MissingTopics { get; set; } = "[]";

    public DateTime CreatedAt { get; set; }

    public string AIModel { get; set; } = string.Empty;

    public string PromptVersion { get; set; } = string.Empty;
}