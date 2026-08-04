using ResumeEntity = InterviewAce.Domain.Entities.Resume;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IResumeRepository
{
    Task AddAsync(ResumeEntity resume);

    Task<List<ResumeEntity>> GetByUserIdAsync(Guid userId);

    Task<ResumeEntity?> GetByIdAsync(Guid id);

    void Delete(ResumeEntity resume);

    Task SaveChangesAsync();

    Task<int> GetResumeCountAsync(Guid userId);
}