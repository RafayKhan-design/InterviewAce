namespace InterviewAce.Application.DTOs.ResumeAnalysis;

public class ResumeAnalysisResponseDto
{
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public string ExtractedText { get; set; } = string.Empty;


    public List<string> Skills { get; set; } = new();

    public List<string> Experience { get; set; } = new();

    public List<string> Education { get; set; } = new();

    public List<string> Projects { get; set; } = new();

    public List<string> Certifications { get; set; } = new();

    public List<string> Strengths { get; set; } = new();

    public List<string> Weaknesses { get; set; } = new();


    public int ResumeScore { get; set; }


    public DateTime CreatedAt { get; set; }
}