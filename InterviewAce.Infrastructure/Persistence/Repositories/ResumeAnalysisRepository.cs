using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class ResumeAnalysisRepository : IResumeAnalysisRepository
{
    private readonly ApplicationDbContext _context;


    public ResumeAnalysisRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task AddAsync(
        ResumeAnalysis analysis)
    {
        await _context.ResumeAnalyses
            .AddAsync(analysis);
    }



    public async Task<ResumeAnalysis?> GetByResumeIdAsync(
        Guid resumeId)
    {
        return await _context.ResumeAnalyses
            .FirstOrDefaultAsync(
                x => x.ResumeId == resumeId
            );
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