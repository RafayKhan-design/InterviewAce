namespace InterviewAce.Application.DTOs.Interview;

public class GenerateInterviewResponseDto
{
    public Guid Id { get; set; }

    public Guid ResumeAnalysisId { get; set; }

    public Guid JobDescriptionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string InterviewType { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public int QuestionCount { get; set; }

    public List<InterviewQuestionDto> Questions { get; set; }
        = new();

    public DateTime CreatedAt { get; set; }
}

public class InterviewQuestionDto
{
    public Guid Id { get; set; }

    public int Order { get; set; }

    public string Question { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;

    public List<string> ExpectedTopics { get; set; }
        = new();

    public string Difficulty { get; set; } = string.Empty;
}