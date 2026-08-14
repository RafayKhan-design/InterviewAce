using InterviewAce.Application.DTOs.Interview;

namespace InterviewAce.Application.Interfaces.Interview;

public interface IInterviewSessionService
{
    Task<InterviewSessionResponseDto> StartAsync(
        Guid userId,
        Guid interviewId);

    Task<InterviewSessionResponseDto?> GetByIdAsync(
        Guid userId,
        Guid sessionId);

    Task<List<InterviewSessionResponseDto>> GetAllAsync(
        Guid userId);
}