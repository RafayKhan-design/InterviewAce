using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;

[ApiController]
[Route("api/interview-answer-evaluations")]
[Authorize]
public class InterviewAnswerEvaluationController : ControllerBase
{
    private readonly IAnswerEvaluationService _evaluationService;

    public InterviewAnswerEvaluationController(
        IAnswerEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    /// <summary>
    /// Evaluate a submitted interview answer using AI.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Evaluate(
        SubmitAnswerEvaluationDto request)
    {
        var result = await _evaluationService.EvaluateAsync(
            GetUserId(),
            request);

        return Ok(new ApiResponseDto<AnswerEvaluationResponseDto>
        {
            Success = true,
            Message = "Interview answer evaluated successfully.",
            Data = result
        });
    }


    /// <summary>
    /// Get the AI evaluation for a submitted interview answer.
    /// </summary>
    [HttpGet("{interviewAnswerId:guid}")]
    public async Task<IActionResult> GetEvaluation(
        Guid interviewAnswerId)
    {
        var result = await _evaluationService.GetByAnswerIdAsync(
            GetUserId(),
            interviewAnswerId);

        if (result == null)
        {
            return NotFound(new ApiResponseDto<AnswerEvaluationResponseDto>
            {
                Success = false,
                Message = "Interview answer evaluation not found.",
                Data = null
            });
        }

        return Ok(new ApiResponseDto<AnswerEvaluationResponseDto>
        {
            Success = true,
            Message = "Interview answer evaluation retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("{interviewAnswerId:guid}/history")]
    public async Task<IActionResult> GetHistory(
    Guid interviewAnswerId)
    {
        var result = await _evaluationService.GetHistoryByAnswerIdAsync(
            GetUserId(),
            interviewAnswerId);

        return Ok(new ApiResponseDto<List<AnswerEvaluationResponseDto>>
        {
            Success = true,
            Message = "Interview answer evaluation history retrieved successfully.",
            Data = result
        });
    }


    [HttpGet("{interviewAnswerId:guid}/progress")]
    public async Task<IActionResult> GetProgress(
    Guid interviewAnswerId)
    {
        var result = await _evaluationService.GetProgressByAnswerIdAsync(
            GetUserId(),
            interviewAnswerId);

        return Ok(new ApiResponseDto<AnswerEvaluationProgressDto>
        {
            Success = true,
            Message = "Interview answer evaluation progress retrieved successfully.",
            Data = result
        });
    }
}