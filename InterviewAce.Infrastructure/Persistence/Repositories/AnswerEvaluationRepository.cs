using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class AnswerEvaluationRepository : IAnswerEvaluationRepository
{
    private readonly ApplicationDbContext _context;

    public AnswerEvaluationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        AnswerEvaluation evaluation)
    {
        await _context.AnswerEvaluations.AddAsync(evaluation);
    }

    public async Task<List<AnswerEvaluation>> GetByAnswerIdAsync(
     Guid interviewAnswerId)
    {
        return await _context.AnswerEvaluations
            .Where(x => x.InterviewAnswerId == interviewAnswerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<AnswerEvaluation?> GetLatestByAnswerIdAsync(
        Guid interviewAnswerId)
    {
        return await _context.AnswerEvaluations
            .Where(x => x.InterviewAnswerId == interviewAnswerId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}