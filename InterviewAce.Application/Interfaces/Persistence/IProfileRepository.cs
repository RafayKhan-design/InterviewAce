using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IProfileRepository
{
    Task<CandidateProfile?> GetByUserIdAsync(Guid userId);

    Task AddAsync(CandidateProfile profile);

    Task SaveChangesAsync();
}