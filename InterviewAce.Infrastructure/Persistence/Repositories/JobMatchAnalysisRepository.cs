using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class JobMatchAnalysisRepository : IJobMatchAnalysisRepository
{
    private readonly ApplicationDbContext _context;

    public JobMatchAnalysisRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JobMatchAnalysis analysis)
    {
        await _context.JobMatchAnalyses.AddAsync(analysis);
    }

    public async Task<JobMatchAnalysis?> GetByIdAndUserIdAsync(
        Guid id,
        Guid userId)
    {
        return await _context.JobMatchAnalyses
            .Include(x => x.JobDescription)
            .Include(x => x.ResumeAnalysis)
            .ThenInclude(x => x.Resume)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.JobDescription.UserId == userId);
    }

    public async Task<List<JobMatchAnalysis>> GetByUserIdAsync(
    Guid userId)
    {
        return await _context.JobMatchAnalyses
            .Include(x => x.JobDescription)
            .Include(x => x.ResumeAnalysis)
            .ThenInclude(x => x.Resume)
            .Where(x => x.JobDescription.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ResumeAnalysisEntity?> GetByIdAsync(
    Guid id,
    Guid userId)
    {
        return await _context.ResumeAnalyses
            .Include(x => x.Resume)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Resume.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}