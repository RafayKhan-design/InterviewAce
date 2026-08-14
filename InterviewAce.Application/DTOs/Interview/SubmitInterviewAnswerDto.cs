namespace InterviewAce.Application.DTOs.Interview;

public class SubmitInterviewAnswerDto
{
    public Guid InterviewQuestionId { get; set; }

    public string AnswerText { get; set; } = string.Empty;
}