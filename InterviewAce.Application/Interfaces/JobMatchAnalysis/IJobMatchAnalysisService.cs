using InterviewAce.Application.DTOs.JobMatchAnalysis;

namespace InterviewAce.Application.Interfaces.JobMatchAnalysis;

public interface IJobMatchAnalysisService
{
    Task<JobMatchAnalysisResponseDto> AnalyzeAsync(
        Guid userId,
        AnalyzeJobMatchRequestDto request);

    Task<JobMatchAnalysisResponseDto?> GetByIdAsync(
        Guid userId,
        Guid id);

    Task<List<JobMatchAnalysisResponseDto>> GetAllAsync(
        Guid userId);
}