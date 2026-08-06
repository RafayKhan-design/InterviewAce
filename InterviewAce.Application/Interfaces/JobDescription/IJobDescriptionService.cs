using InterviewAce.Application.DTOs.JobDescription;

namespace InterviewAce.Application.Interfaces.JobDescription;

public interface IJobDescriptionService
{
    Task<JobDescriptionResponseDto> CreateAsync(
        Guid userId,
        CreateJobDescriptionDto request);

    Task<List<JobDescriptionResponseDto>> GetAllAsync(
        Guid userId);

    Task<JobDescriptionResponseDto?> GetByIdAsync(
        Guid userId,
        Guid jobDescriptionId);

    Task<JobDescriptionResponseDto?> UpdateAsync(
        Guid userId,
        Guid jobDescriptionId,
        UpdateJobDescriptionDto request);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid jobDescriptionId);
}