using InterviewAce.Application.DTOs.ResumeAnalysis;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Application.Interfaces.ResumeAnalysis;
using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;

namespace InterviewAce.Application.Services.ResumeAnalysis;

public class ResumeAnalysisService : IResumeAnalysisService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeAnalysisRepository _resumeAnalysisRepository;


    public ResumeAnalysisService(
        IResumeRepository resumeRepository,
        IResumeAnalysisRepository resumeAnalysisRepository)
    {
        _resumeRepository = resumeRepository;
        _resumeAnalysisRepository = resumeAnalysisRepository;
    }



    public async Task<ResumeAnalysisResponseDto> AnalyzeAsync(
        Guid userId,
        AnalyzeResumeRequestDto request)
    {
        // Check resume exists and belongs to user
        var resume = await _resumeRepository
            .GetByIdAsync(request.ResumeId);


        if (resume == null || resume.UserId != userId)
        {
            throw new InvalidOperationException(
                "Resume not found."
            );
        }



        // Prevent duplicate analysis
        var existingAnalysis = await _resumeAnalysisRepository
            .GetByResumeIdAsync(request.ResumeId);


        if (existingAnalysis != null)
        {
            return MapToResponse(existingAnalysis);
        }



        /*
         Future Flow:

         Resume File
              |
              |
         Text Extraction
              |
              |
         AI Analysis
              |
              |
         Save Structured Result

        */


        var analysis = new ResumeAnalysisEntity
        {
            Id = Guid.NewGuid(),

            ResumeId = resume.Id,

            ExtractedText = string.Empty,

            Skills = null,

            Experience = null,

            Education = null,

            Projects = null,

            Certifications = null,

            Strengths = null,

            Weaknesses = null,

            ResumeScore = 0,

            CreatedAt = DateTime.UtcNow
        };



        await _resumeAnalysisRepository
            .AddAsync(analysis);


        await _resumeAnalysisRepository
            .SaveChangesAsync();



        return MapToResponse(analysis);
    }





    public async Task<ResumeAnalysisResponseDto?> GetAnalysisAsync(
        Guid userId,
        Guid resumeId)
    {
        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);


        if (resume == null || resume.UserId != userId)
        {
            return null;
        }



        var analysis = await _resumeAnalysisRepository
            .GetByResumeIdAsync(resumeId);



        if (analysis == null)
        {
            return null;
        }



        return MapToResponse(analysis);
    }





    private static ResumeAnalysisResponseDto MapToResponse(
        ResumeAnalysisEntity analysis)
    {
        return new ResumeAnalysisResponseDto
        {
            Id = analysis.Id,

            ResumeId = analysis.ResumeId,

            ExtractedText = analysis.ExtractedText,

            Skills = analysis.Skills,

            Experience = analysis.Experience,

            Education = analysis.Education,

            Projects = analysis.Projects,

            Certifications = analysis.Certifications,

            Strengths = analysis.Strengths,

            Weaknesses = analysis.Weaknesses,

            ResumeScore = analysis.ResumeScore,

            CreatedAt = analysis.CreatedAt
        };
    }
}