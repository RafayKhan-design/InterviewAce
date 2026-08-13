using InterviewEntity = InterviewAce.Domain.Entities.Interview;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IInterviewRepository
{
    Task AddAsync(
        InterviewEntity interview);

    Task<InterviewEntity?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId);

    Task<List<InterviewEntity>> GetByUserIdAsync(
        Guid userId);

    Task SaveChangesAsync();
}