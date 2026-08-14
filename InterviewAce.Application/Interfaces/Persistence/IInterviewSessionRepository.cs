using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IInterviewSessionRepository
{
    Task AddAsync(InterviewSession session);

    Task<InterviewSession?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId);

    Task<List<InterviewSession>> GetByUserIdAsync(
        Guid userId);

    Task SaveChangesAsync();
}