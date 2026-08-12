using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.JobMatchAnalysis;
using InterviewAce.Application.Interfaces.JobMatchAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/job-match-analysis")]
[Authorize]
public class JobMatchAnalysisController : ControllerBase
{
    private readonly IJobMatchAnalysisService _service;

    public JobMatchAnalysisController(
        IJobMatchAnalysisService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    /// <summary>
    /// Analyze a resume against a job description using AI.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Analyze(
        AnalyzeJobMatchRequestDto request)
    {
        var result = await _service.AnalyzeAsync(
            GetUserId(),
            request);

        return Ok(new ApiResponseDto<JobMatchAnalysisResponseDto>
        {
            Success = true,
            Message = "Job match analysis completed successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get all job match analyses of the logged-in user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(
            GetUserId());

        return Ok(
            new ApiResponseDto<List<JobMatchAnalysisResponseDto>>
            {
                Success = true,
                Message = "Job match analyses retrieved successfully.",
                Data = result
            });
    }

    /// <summary>
    /// Get a job match analysis by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(
            GetUserId(),
            id);

        if (result == null)
        {
            return NotFound(
                new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Job match analysis not found."
                });
        }

        return Ok(
            new ApiResponseDto<JobMatchAnalysisResponseDto>
            {
                Success = true,
                Message = "Job match analysis retrieved successfully.",
                Data = result
            });
    }
}