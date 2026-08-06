using InterviewAce.API.Controllers.Base;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Resume;
using InterviewAce.Application.Interfaces.Resume;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

/// <summary>
/// Handles resume upload, retrieval, download, and deletion operations for authenticated users.
/// </summary>
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



    /// <summary>
    /// Uploads a new resume for the authenticated user.
    /// </summary>
    /// <param name="request">Resume file upload request.</param>
    /// <returns>Uploaded resume information.</returns>
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



    /// <summary>
    /// Retrieves all resumes uploaded by the authenticated user.
    /// </summary>
    /// <returns>List of user's resumes.</returns>
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



    /// <summary>
    /// Deletes a resume owned by the authenticated user.
    /// </summary>
    /// <param name="id">Resume unique identifier.</param>
    /// <returns>Deletion status.</returns>
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



    /// <summary>
    /// Downloads a resume file owned by the authenticated user.
    /// </summary>
    /// <param name="id">Resume unique identifier.</param>
    /// <returns>Resume file.</returns>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(
        Guid id)
    {
        var result = await _resumeService
            .DownloadAsync(CurrentUserId, id);


        if (result == null)
        {
            return NotFound(
                new ApiResponseDto<object>(
                    false,
                    "Resume not found."
                )
            );
        }


        return File(
            result.Value.FileBytes,
            result.Value.FileType,
            result.Value.FileName
        );
    }
}