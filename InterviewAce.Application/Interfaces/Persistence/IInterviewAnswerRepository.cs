using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IInterviewAnswerRepository
{
    Task AddAsync(InterviewAnswer answer);

    Task<InterviewAnswer?> GetByIdAsync(
        Guid id,
        Guid userId);

    Task<List<InterviewAnswer>> GetBySessionIdAsync(
        Guid sessionId,
        Guid userId);

    Task SaveChangesAsync();
}