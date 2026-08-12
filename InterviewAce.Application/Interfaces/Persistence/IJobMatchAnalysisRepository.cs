using JobMatchAnalysisEntity = InterviewAce.Domain.Entities.JobMatchAnalysis;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IJobMatchAnalysisRepository
{
    Task AddAsync(JobMatchAnalysisEntity analysis);

    Task<JobMatchAnalysisEntity?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId);

    Task<List<JobMatchAnalysisEntity>> GetByUserIdAsync(
        Guid userId);

    Task SaveChangesAsync();
}