using InterviewAce.API.Controllers.Base;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Resume;
using InterviewAce.Application.Interfaces;
using InterviewAce.Application.Interfaces.Resume;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;


[Authorize]
[Route("api/[controller]")]
public class ResumeController : BaseApiController
{
    private readonly IResumeService _resumeService;


    public ResumeController(
        IResumeService resumeService)
    {
        _resumeService = resumeService;
    }



    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] UploadResumeDto request)
    {
        var resume = await _resumeService
            .UploadAsync(CurrentUserId, request);


        return Ok(
            new ApiResponseDto<ResumeResponseDto>(
                true,
                "Resume uploaded successfully.",
                resume
            )
        );
    }



    [HttpGet]
    public async Task<IActionResult> GetMyResumes()
    {
        var resumes = await _resumeService
            .GetMyResumesAsync(CurrentUserId);


        return Ok(
            new ApiResponseDto<List<ResumeResponseDto>>(
                true,
                "Resumes retrieved successfully.",
                resumes
            )
        );
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        var result = await _resumeService
            .DeleteAsync(CurrentUserId, id);


        if (!result)
        {
            return NotFound(
                new ApiResponseDto<object>(
                    false,
                    "Resume not found."
                )
            );
        }


        return Ok(
            new ApiResponseDto<object>(
                true,
                "Resume deleted successfully."
            )
        );
    }
}