using InterviewAce.Application.DTOs.Profile;

namespace InterviewAce.Application.Interfaces;

public interface IProfileService
{
    Task<ProfileResponseDto?> CreateAsync(
        Guid userId,
        CreateProfileDto request);


    Task<ProfileResponseDto?> GetMyProfileAsync(
        Guid userId);


    Task<ProfileResponseDto?> UpdateAsync(
        Guid userId,
        CreateProfileDto request);
}