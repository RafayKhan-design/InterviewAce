namespace InterviewAce.Infrastructure.Services.AI.Models;

public class ResumeAIResponse
{
    public string? CandidateLevel { get; set; }

    public string? Summary { get; set; }


    public List<string> Skills { get; set; } = new();


    public List<string> TechnicalSkills { get; set; } = new();


    public List<string> SoftSkills { get; set; } = new();


    public List<string> Experience { get; set; } = new();


    public List<string> Education { get; set; } = new();


    public List<string> Projects { get; set; } = new();


    public List<string> Certifications { get; set; } = new();


    public List<string> Strengths { get; set; } = new();


    public List<string> Weaknesses { get; set; } = new();


    public List<string> MissingSkills { get; set; } = new();


    public List<string> RecommendedLearning { get; set; } = new();


    public int AtsScore { get; set; }


    public int ResumeScore { get; set; }


    public int InterviewReadinessScore { get; set; }
}