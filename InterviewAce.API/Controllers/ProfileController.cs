using InterviewAce.API.Controllers.Base;
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
                "Profile already exists.");
        }


        return Ok(profile);
    }



    [HttpGet]
    [Route("api/profile/me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _profileService
            .GetMyProfileAsync(CurrentUserId);


        if (profile == null)
        {
            return NotFound();
        }


        return Ok(profile);
    }



    [HttpPut]
    [Route("api/profile")]
    public async Task<IActionResult> Update(
        CreateProfileDto request)
    {
        var profile = await _profileService
            .UpdateAsync(CurrentUserId, request);


        if (profile == null)
        {
            return NotFound();
        }


        return Ok(profile);
    }
}