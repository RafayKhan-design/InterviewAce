namespace InterviewAce.Application.DTOs.Interview;

public class GenerateInterviewRequestDto
{
    public Guid ResumeAnalysisId { get; set; }

    public Guid JobDescriptionId { get; set; }

    public string InterviewType { get; set; } = "Technical";

    public string Difficulty { get; set; } = "Intermediate";

    public int QuestionCount { get; set; } = 10;
}