using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class JobDescriptionRepository
    : IJobDescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public JobDescriptionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JobDescription jobDescription)
    {
        await _context.JobDescriptions.AddAsync(jobDescription);
    }

    public async Task<List<JobDescription>> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.JobDescriptions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<JobDescription?> GetByIdAsync(Guid id)
    {
        return await _context.JobDescriptions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<JobDescription?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId)
    {
        return await _context.JobDescriptions
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Remove(JobDescription jobDescription)
    {
        _context.JobDescriptions.Remove(jobDescription);
    }
}