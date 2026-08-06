namespace InterviewAce.Application.DTOs.ResumeAnalysis;

public class ResumeAnalysisResponseDto
{
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }


    public string ExtractedText { get; set; } = string.Empty;


    public string? Skills { get; set; }


    public string? Experience { get; set; }


    public string? Education { get; set; }


    public string? Projects { get; set; }


    public string? Certifications { get; set; }


    public string? Strengths { get; set; }


    public string? Weaknesses { get; set; }


    public int ResumeScore { get; set; }


    public DateTime CreatedAt { get; set; }
}