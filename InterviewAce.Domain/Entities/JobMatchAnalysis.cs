namespace InterviewAce.Domain.Entities;

public class JobMatchAnalysis
{
    public Guid Id { get; set; }

    public Guid ResumeAnalysisId { get; set; }

    public ResumeAnalysis ResumeAnalysis { get; set; } = null!;

    public Guid JobDescriptionId { get; set; }

    public JobDescription JobDescription { get; set; } = null!;

    public int MatchScore { get; set; }

    public string MatchingSkills { get; set; } = "[]";

    public string MissingSkills { get; set; } = "[]";

    public string ExperienceMatch { get; set; } = string.Empty;

    public string Strengths { get; set; } = "[]";

    public string Gaps { get; set; } = "[]";

    public string Recommendations { get; set; } = "[]";

    public DateTime CreatedAt { get; set; }
}