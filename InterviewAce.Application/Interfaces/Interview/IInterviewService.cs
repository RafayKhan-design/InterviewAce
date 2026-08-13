using InterviewAce.Application.DTOs.Interview;

namespace InterviewAce.Application.Interfaces.Interview;

public interface IInterviewService
{
    Task<GenerateInterviewResponseDto> GenerateAsync(
        Guid userId,
        GenerateInterviewRequestDto request);

    Task<GenerateInterviewResponseDto?> GetByIdAsync(
        Guid userId,
        Guid interviewId);

    Task<List<GenerateInterviewResponseDto>> GetAllAsync(
        Guid userId);
}