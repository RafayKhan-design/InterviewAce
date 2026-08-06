using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IResumeAnalysisRepository
{
    Task AddAsync(
        ResumeAnalysisEntity analysis);


    Task<ResumeAnalysisEntity?> GetByResumeIdAsync(
        Guid resumeId);


    Task SaveChangesAsync();
}