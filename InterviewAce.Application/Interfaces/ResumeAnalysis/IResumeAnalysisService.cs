using InterviewAce.Application.DTOs.ResumeAnalysis;

namespace InterviewAce.Application.Interfaces.ResumeAnalysis;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResponseDto> AnalyzeAsync(
        Guid userId,
        AnalyzeResumeRequestDto request);


    Task<ResumeAnalysisResponseDto?> GetAnalysisAsync(
        Guid userId,
        Guid resumeId);
}