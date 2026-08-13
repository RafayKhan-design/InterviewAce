using InterviewAce.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

using InterviewEntity = InterviewAce.Domain.Entities.Interview;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        InterviewEntity interview)
    {
        await _context.Interviews.AddAsync(interview);
    }

    public async Task<InterviewEntity?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId)
    {
        return await _context.Interviews
            .Include(x => x.Questions)
            .Include(x => x.ResumeAnalysis)
            .Include(x => x.JobDescription)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);
    }

    public async Task<List<InterviewEntity>> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.Interviews
            .Include(x => x.Questions)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}