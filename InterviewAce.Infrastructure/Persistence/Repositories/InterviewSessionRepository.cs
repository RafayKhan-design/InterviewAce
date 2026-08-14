using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class InterviewSessionRepository : IInterviewSessionRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewSessionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        InterviewSession session)
    {
        await _context.InterviewSessions.AddAsync(session);
    }

    public async Task<InterviewSession?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId)
    {
        return await _context.InterviewSessions
    .Include(x => x.Answers)
    .Include(x => x.Interview)
        .ThenInclude(x => x.Questions)
    .FirstOrDefaultAsync(x =>
        x.Id == id &&
        x.UserId == userId);
    }

    public async Task<List<InterviewSession>> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.InterviewSessions
            .Include(x => x.Answers)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}