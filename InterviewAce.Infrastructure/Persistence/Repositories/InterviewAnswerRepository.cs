using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class InterviewAnswerRepository : IInterviewAnswerRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewAnswerRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        InterviewAnswer answer)
    {
        await _context.InterviewAnswers.AddAsync(answer);
    }

    public async Task<InterviewAnswer?> GetByIdAsync(
        Guid id,
        Guid userId)
    {
        return await _context.InterviewAnswers
            .Include(a => a.InterviewSession)
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.InterviewSession.Interview.UserId == userId);
    }

    public async Task<List<InterviewAnswer>> GetBySessionIdAsync(
        Guid sessionId,
        Guid userId)
    {
        return await _context.InterviewAnswers
            .Include(a => a.InterviewSession)
            .Where(a =>
                a.InterviewSessionId == sessionId &&
                a.InterviewSession.Interview.UserId == userId)
            .OrderBy(a => a.AnsweredAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}