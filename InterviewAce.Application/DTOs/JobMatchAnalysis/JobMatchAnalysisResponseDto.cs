namespace InterviewAce.Application.DTOs.JobMatchAnalysis;

public class JobMatchAnalysisResponseDto
{
    public Guid Id { get; set; }

    public Guid ResumeAnalysisId { get; set; }

    public Guid JobDescriptionId { get; set; }

    public int MatchScore { get; set; }

    public List<string> MatchingSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();

    public string ExperienceMatch { get; set; } = string.Empty;

    public List<string> Strengths { get; set; } = new();

    public List<string> Gaps { get; set; } = new();

    public List<string> Recommendations { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}