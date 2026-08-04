using InterviewAce.Application.DTOs.Profile;
using InterviewAce.Application.Interfaces;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Services.Profile;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ProfileResponseDto?> CreateAsync(
        Guid userId,
        CreateProfileDto request)
    {
        var existingProfile = await _profileRepository
            .GetByUserIdAsync(userId);

        if (existingProfile != null)
        {
            return null;
        }

        var profile = new CandidateProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName,
            Phone = request.Phone,
            LinkedInUrl = request.LinkedInUrl,
            GitHubUrl = request.GitHubUrl,
            Bio = request.Bio,
            YearsOfExperience = request.YearsOfExperience,
            Education = request.Education,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.AddAsync(profile);
        await _profileRepository.SaveChangesAsync();

        return new ProfileResponseDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Phone = profile.Phone,
            LinkedInUrl = profile.LinkedInUrl,
            GitHubUrl = profile.GitHubUrl,
            Bio = profile.Bio,
            YearsOfExperience = profile.YearsOfExperience,
            Education = profile.Education,
            CreatedAt = profile.CreatedAt
        };
    }

    public async Task<ProfileResponseDto?> GetMyProfileAsync(Guid userId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);

        if (profile == null)
        {
            return null;
        }

        return new ProfileResponseDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Phone = profile.Phone,
            LinkedInUrl = profile.LinkedInUrl,
            GitHubUrl = profile.GitHubUrl,
            Bio = profile.Bio,
            YearsOfExperience = profile.YearsOfExperience,
            Education = profile.Education,
            CreatedAt = profile.CreatedAt
        };
    }

    public async Task<ProfileResponseDto?> UpdateAsync(
        Guid userId,
        UpdateProfileDto request)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);

        if (profile == null)
        {
            return null;
        }

        profile.FullName = request.FullName;
        profile.Phone = request.Phone;
        profile.LinkedInUrl = request.LinkedInUrl;
        profile.GitHubUrl = request.GitHubUrl;
        profile.Bio = request.Bio;
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.Education = request.Education;
        profile.UpdatedAt = DateTime.UtcNow;

        await _profileRepository.SaveChangesAsync();

        return new ProfileResponseDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Phone = profile.Phone,
            LinkedInUrl = profile.LinkedInUrl,
            GitHubUrl = profile.GitHubUrl,
            Bio = profile.Bio,
            YearsOfExperience = profile.YearsOfExperience,
            Education = profile.Education,
            CreatedAt = profile.CreatedAt
        };
    }
}