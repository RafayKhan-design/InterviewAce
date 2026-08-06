using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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



    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}