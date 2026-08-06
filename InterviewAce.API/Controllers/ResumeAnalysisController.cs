using InterviewAce.API.Controllers.Base;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.DTOs.ResumeAnalysis;
using InterviewAce.Application.Interfaces.ResumeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers;


[Authorize]
[Route("api/[controller]")]
public class ResumeAnalysisController : BaseApiController
{
    private readonly IResumeAnalysisService _resumeAnalysisService;


    public ResumeAnalysisController(
        IResumeAnalysisService resumeAnalysisService)
    {
        _resumeAnalysisService = resumeAnalysisService;
    }



    /// <summary>
    /// Analyze uploaded resume.
    /// </summary>
    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(
        [FromBody] AnalyzeResumeRequestDto request)
    {
        var result = await _resumeAnalysisService
            .AnalyzeAsync(
                CurrentUserId,
                request
            );


        return Ok(
            new ApiResponseDto<ResumeAnalysisResponseDto>(
                true,
                "Resume analyzed successfully.",
                result
            )
        );
    }




    /// <summary>
    /// Get resume analysis result.
    /// </summary>
    [HttpGet("{resumeId}")]
    public async Task<IActionResult> GetAnalysis(
        Guid resumeId)
    {
        var result = await _resumeAnalysisService
            .GetAnalysisAsync(
                CurrentUserId,
                resumeId
            );


        if (result == null)
        {
            return NotFound(
                new ApiResponseDto<object>(
                    false,
                    "Resume analysis not found."
                )
            );
        }


        return Ok(
            new ApiResponseDto<ResumeAnalysisResponseDto>(
                true,
                "Resume analysis retrieved successfully.",
                result
            )
        );
    }
}