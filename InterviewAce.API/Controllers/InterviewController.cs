using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/interviews")]
[Authorize]
public class InterviewController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewController(
        IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    /// <summary>
    /// Generate a personalized AI interview.
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        GenerateInterviewRequestDto request)
    {
        var result = await _interviewService.GenerateAsync(
            GetUserId(),
            request);

        return Ok(new ApiResponseDto<GenerateInterviewResponseDto>
        {
            Success = true,
            Message = "Interview generated successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get all interviews of the logged-in user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _interviewService.GetAllAsync(
            GetUserId());

        return Ok(
            new ApiResponseDto<List<GenerateInterviewResponseDto>>
            {
                Success = true,
                Message = "Interviews retrieved successfully.",
                Data = result
            });
    }

    /// <summary>
    /// Get an interview by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _interviewService.GetByIdAsync(
            GetUserId(),
            id);

        if (result == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Interview not found."
            });
        }

        return Ok(
            new ApiResponseDto<GenerateInterviewResponseDto>
            {
                Success = true,
                Message = "Interview retrieved successfully.",
                Data = result
            });
    }
}