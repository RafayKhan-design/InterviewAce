using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.JobDescription;
using InterviewAce.Application.Interfaces.JobDescription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/job-descriptions")]
[Authorize]
public class JobDescriptionController : ControllerBase
{
    private readonly IJobDescriptionService _jobDescriptionService;

    public JobDescriptionController(
        IJobDescriptionService jobDescriptionService)
    {
        _jobDescriptionService = jobDescriptionService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );
    }

    /// <summary>
    /// Create a new job description.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobDescriptionDto request)
    {
        var result = await _jobDescriptionService.CreateAsync(
            GetUserId(),
            request);

        return Ok(new ApiResponseDto<JobDescriptionResponseDto>
        {
            Success = true,
            Message = "Job description created successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get all job descriptions of the logged-in user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _jobDescriptionService.GetAllAsync(
            GetUserId());

        return Ok(new ApiResponseDto<List<JobDescriptionResponseDto>>
        {
            Success = true,
            Message = "Job descriptions retrieved successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get a job description by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _jobDescriptionService.GetByIdAsync(
            GetUserId(),
            id);

        if (result == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Job description not found."
            });
        }

        return Ok(new ApiResponseDto<JobDescriptionResponseDto>
        {
            Success = true,
            Message = "Job description retrieved successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Update a job description.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateJobDescriptionDto request)
    {
        var result = await _jobDescriptionService.UpdateAsync(
            GetUserId(),
            id,
            request);

        if (result == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Job description not found."
            });
        }

        return Ok(new ApiResponseDto<JobDescriptionResponseDto>
        {
            Success = true,
            Message = "Job description updated successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Delete a job description.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _jobDescriptionService.DeleteAsync(
            GetUserId(),
            id);

        if (!deleted)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Job description not found."
            });
        }

        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Message = "Job description deleted successfully."
        });
    }
}