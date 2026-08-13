namespace InterviewAce.Application.DTOs.Interview;

public class InterviewAIResponse
{
    public string Title { get; set; } = string.Empty;

    public List<InterviewAIQuestion> Questions { get; set; }
        = new();
}

public class InterviewAIQuestion
{
    public int Order { get; set; }

    public string Question { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;

    public List<string> ExpectedTopics { get; set; }
        = new();

    public string Difficulty { get; set; } = string.Empty;
}