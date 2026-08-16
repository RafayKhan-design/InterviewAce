using InterviewAce.Application.DTOs.Interview;

namespace InterviewAce.Application.Interfaces.Interview;

public interface IAnswerEvaluationService
{
    Task<AnswerEvaluationResponseDto> EvaluateAsync(
        Guid userId,
        SubmitAnswerEvaluationDto request);

    Task<AnswerEvaluationResponseDto?> GetByAnswerIdAsync(
    Guid userId,
    Guid interviewAnswerId);
}