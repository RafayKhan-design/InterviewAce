namespace InterviewAce.Domain.Entities;

public class InterviewQuestion
{
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }

    public Interview Interview { get; set; } = null!;

    public int Order { get; set; }

    public string Question { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;

    public string ExpectedTopics { get; set; } = "[]";

    public string Difficulty { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}