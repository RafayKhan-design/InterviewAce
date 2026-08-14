using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/interview-sessions/{sessionId:guid}/answers")]
[Authorize]
public class InterviewAnswerController : ControllerBase
{
    private readonly IInterviewAnswerService _answerService;

    public InterviewAnswerController(
        IInterviewAnswerService answerService)
    {
        _answerService = answerService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    /// <summary>
    /// Submit an answer to an interview question.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit(
        Guid sessionId,
        SubmitInterviewAnswerDto request)
    {
        var result = await _answerService.SubmitAsync(
            GetUserId(),
            sessionId,
            request);

        return Ok(new ApiResponseDto<InterviewAnswerResponseDto>
        {
            Success = true,
            Message = "Interview answer submitted successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Get all answers submitted during an interview session.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBySessionId(
        Guid sessionId)
    {
        var result = await _answerService.GetBySessionIdAsync(
            GetUserId(),
            sessionId);

        return Ok(
            new ApiResponseDto<List<InterviewAnswerResponseDto>>
            {
                Success = true,
                Message = "Interview answers retrieved successfully.",
                Data = result
            });
    }
}