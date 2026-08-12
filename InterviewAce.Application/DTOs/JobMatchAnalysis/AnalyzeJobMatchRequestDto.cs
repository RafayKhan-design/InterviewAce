namespace InterviewAce.Application.DTOs.JobMatchAnalysis;

public class AnalyzeJobMatchRequestDto
{
    public Guid ResumeAnalysisId { get; set; }

    public Guid JobDescriptionId { get; set; }
}