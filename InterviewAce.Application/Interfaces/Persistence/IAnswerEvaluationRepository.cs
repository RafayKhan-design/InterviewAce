using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IAnswerEvaluationRepository
{
    Task AddAsync(AnswerEvaluation evaluation);

    Task<List<AnswerEvaluation>> GetByAnswerIdAsync(
        Guid interviewAnswerId);

    Task<AnswerEvaluation?> GetLatestByAnswerIdAsync(
        Guid interviewAnswerId);

    Task SaveChangesAsync();
}