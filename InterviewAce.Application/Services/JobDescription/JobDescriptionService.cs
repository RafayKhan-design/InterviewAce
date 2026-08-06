using InterviewAce.Application.DTOs.JobDescription;
using InterviewAce.Application.Interfaces.JobDescription;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Services.JobDescription;

public class JobDescriptionService : IJobDescriptionService
{
    private readonly IJobDescriptionRepository _repository;

    public JobDescriptionService(
        IJobDescriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<JobDescriptionResponseDto> CreateAsync(
        Guid userId,
        CreateJobDescriptionDto request)
    {
        var jobDescription = new Domain.Entities.JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            CompanyName = request.CompanyName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(jobDescription);
        await _repository.SaveChangesAsync();

        return MapToResponse(jobDescription);
    }

    public async Task<List<JobDescriptionResponseDto>> GetAllAsync(
        Guid userId)
    {
        var jobs = await _repository.GetByUserIdAsync(userId);

        return jobs
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<JobDescriptionResponseDto?> GetByIdAsync(
        Guid userId,
        Guid jobDescriptionId)
    {
        var job = await _repository.GetByIdAndUserIdAsync(
            jobDescriptionId,
            userId);

        if (job == null)
        {
            return null;
        }

        return MapToResponse(job);
    }

    public async Task<JobDescriptionResponseDto?> UpdateAsync(
        Guid userId,
        Guid jobDescriptionId,
        UpdateJobDescriptionDto request)
    {
        var job = await _repository.GetByIdAndUserIdAsync(
            jobDescriptionId,
            userId);

        if (job == null)
        {
            return null;
        }

        job.Title = request.Title;
        job.CompanyName = request.CompanyName;
        job.Description = request.Description;
        job.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return MapToResponse(job);
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid jobDescriptionId)
    {
        var job = await _repository.GetByIdAndUserIdAsync(
            jobDescriptionId,
            userId);

        if (job == null)
        {
            return false;
        }

        _repository.Remove(job);

        await _repository.SaveChangesAsync();

        return true;
    }

    private static JobDescriptionResponseDto MapToResponse(
        Domain.Entities.JobDescription job)
    {
        return new JobDescriptionResponseDto
        {
            Id = job.Id,
            Title = job.Title,
            CompanyName = job.CompanyName,
            Description = job.Description,
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt
        };
    }
}