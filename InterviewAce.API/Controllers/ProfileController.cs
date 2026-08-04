using InterviewAce.API.Controllers.Base;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Profile;
using InterviewAce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[Authorize]
public class ProfileController : BaseApiController
{
    private readonly IProfileService _profileService;


    public ProfileController(
        IProfileService profileService)
    {
        _profileService = profileService;
    }


    /// <summary>
    /// Creates a profile for the authenticated user.
    /// </summary>
    /// <param name="request">Profile information.</param>
    /// <returns>The created profile details.</returns>
    [HttpPost]
    [Route("api/profile")]
    public async Task<IActionResult> Create(
        CreateProfileDto request)
    {
        var profile = await _profileService
            .CreateAsync(CurrentUserId, request);


        if (profile == null)
        {
            return BadRequest(
                new ApiResponseDto<object>(
                    false,
                    "Profile already exists."
                )
            );
        }


        return Ok(
            new ApiResponseDto<ProfileResponseDto>(
                true,
                "Profile created successfully.",
                profile
            )
        );
    }


    /// <summary>
    /// Retrieves the authenticated user's profile.
    /// </summary>
    /// <returns>User profile information.</returns>
    [HttpGet]
    [Route("api/profile/me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _profileService
            .GetMyProfileAsync(CurrentUserId);


        if (profile == null)
        {
            return NotFound(
                new ApiResponseDto<object>(
                    false,
                    "Profile not found."
                )
            );
        }


        return Ok(
            new ApiResponseDto<ProfileResponseDto>(
                true,
                "Profile retrieved successfully.",
                profile
            )
        );
    }


    /// <summary>
    /// Updates the authenticated user's profile.
    /// </summary>
    /// <param name="request">Updated profile information.</param>
    /// <returns>The updated profile details.</returns>
    [HttpPut]
    [Route("api/profile")]
    public async Task<IActionResult> Update(
        UpdateProfileDto request)
    {
        var profile = await _profileService
            .UpdateAsync(CurrentUserId, request);


        if (profile == null)
        {
            return NotFound(
                new ApiResponseDto<object>(
                    false,
                    "Profile not found."
                )
            );
        }


        return Ok(
            new ApiResponseDto<ProfileResponseDto>(
                true,
                "Profile updated successfully.",
                profile
            )
        );
    }

   
}