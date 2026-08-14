namespace InterviewAce.Domain.Entities;

public class InterviewAnswer
{
    public Guid Id { get; set; }

    public Guid InterviewSessionId { get; set; }

    public InterviewSession InterviewSession { get; set; } = null!;

    public Guid InterviewQuestionId { get; set; }

    public InterviewQuestion InterviewQuestion { get; set; } = null!;

    public string AnswerText { get; set; } = string.Empty;

    public DateTime AnsweredAt { get; set; }
}