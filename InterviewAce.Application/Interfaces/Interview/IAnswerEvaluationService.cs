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

    Task<List<AnswerEvaluationResponseDto>> GetHistoryByAnswerIdAsync(
    Guid userId,
    Guid interviewAnswerId);

    Task<AnswerEvaluationProgressDto> GetProgressByAnswerIdAsync(
    Guid userId,
    Guid interviewAnswerId);
}