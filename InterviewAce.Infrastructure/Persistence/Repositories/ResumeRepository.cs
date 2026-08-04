using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly ApplicationDbContext _context;


    public ResumeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Resume resume)
    {
        await _context.Resumes.AddAsync(resume);
    }


    public async Task<List<Resume>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Resumes
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync();
    }


    public async Task<Resume?> GetByIdAsync(Guid id)
    {
        return await _context.Resumes
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public void Delete(Resume resume)
    {
        _context.Resumes.Remove(resume);
    }

    public async Task<int> GetResumeCountAsync(Guid userId)
    {
        return await _context.Resumes
            .CountAsync(x => x.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}