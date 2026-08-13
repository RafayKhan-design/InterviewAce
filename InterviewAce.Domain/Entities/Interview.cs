namespace InterviewAce.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid ResumeAnalysisId { get; set; }

    public ResumeAnalysis ResumeAnalysis { get; set; } = null!;

    public Guid JobDescriptionId { get; set; }

    public JobDescription JobDescription { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string InterviewType { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public int QuestionCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<InterviewQuestion> Questions { get; set; }
        = new List<InterviewQuestion>();
}