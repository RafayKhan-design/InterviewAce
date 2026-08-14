using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/interview-sessions")]
[Authorize]
public class InterviewSessionController : ControllerBase
{
    private readonly IInterviewSessionService _sessionService;

    public InterviewSessionController(
        IInterviewSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    /// <summary>
    /// Start a new interview session.
    /// </summary>
    [HttpPost("{interviewId:guid}/start")]
    public async Task<IActionResult> Start(Guid interviewId)
    {
        var result = await _sessionService.StartAsync(
            GetUserId(),
            interviewId);

        return Ok(new ApiResponseDto<InterviewSessionResponseDto>
        {
            Success = true,
            Message = "Interview session started successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get all interview sessions of the logged-in user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sessionService.GetAllAsync(
            GetUserId());

        return Ok(
            new ApiResponseDto<List<InterviewSessionResponseDto>>
            {
                Success = true,
                Message = "Interview sessions retrieved successfully.",
                Data = result
            });
    }

    /// <summary>
    /// Get an interview session by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sessionService.GetByIdAsync(
            GetUserId(),
            id);

        if (result == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Interview session not found."
            });
        }

        return Ok(
            new ApiResponseDto<InterviewSessionResponseDto>
            {
                Success = true,
                Message = "Interview session retrieved successfully.",
                Data = result
            });
    }
}