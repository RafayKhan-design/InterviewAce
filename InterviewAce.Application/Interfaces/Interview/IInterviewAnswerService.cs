using InterviewAce.Application.DTOs.Interview;

namespace InterviewAce.Application.Interfaces.Interview;

public interface IInterviewAnswerService
{
    Task<InterviewAnswerResponseDto> SubmitAsync(
        Guid userId,
        Guid sessionId,
        SubmitInterviewAnswerDto request);

    Task<List<InterviewAnswerResponseDto>> GetBySessionIdAsync(
        Guid userId,
        Guid sessionId);
}