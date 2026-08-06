using JobDescriptionEntity = InterviewAce.Domain.Entities.JobDescription;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IJobDescriptionRepository
{
    Task AddAsync(JobDescriptionEntity jobDescription);

    Task<List<JobDescriptionEntity>> GetByUserIdAsync(Guid userId);

    Task<JobDescriptionEntity?> GetByIdAsync(Guid id);

    Task<JobDescriptionEntity?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId);

    Task SaveChangesAsync();

    void Remove(JobDescriptionEntity jobDescription);
}