using InterviewAce.Application.DTOs.ResumeAnalysis;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Application.Interfaces.Extraction;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Application.Interfaces.Processing;
using InterviewAce.Application.Interfaces.ResumeAnalysis;
using InterviewAce.Application.Interfaces.Storage;
using System.Text.Json;
using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;

namespace InterviewAce.Application.Services.ResumeAnalysis;

public class ResumeAnalysisService : IResumeAnalysisService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeAnalysisRepository _resumeAnalysisRepository;
    private readonly IResumeAnalyzer _resumeAnalyzer;
    private readonly IResumeTextExtractor _resumeTextExtractor;
    private readonly ITextCleaner _textCleaner;

    public ResumeAnalysisService(
    IResumeRepository resumeRepository,
    IResumeAnalysisRepository resumeAnalysisRepository,
    IResumeAnalyzer resumeAnalyzer,
    IResumeTextExtractor resumeTextExtractor,
    ITextCleaner textCleaner)
    {
        _resumeRepository = resumeRepository;
        _resumeAnalysisRepository = resumeAnalysisRepository;
        _resumeAnalyzer = resumeAnalyzer;
        _resumeTextExtractor = resumeTextExtractor;
        _textCleaner = textCleaner;
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
        //var existingAnalysis = await _resumeAnalysisRepository
        //    .GetByResumeIdAsync(request.ResumeId);


        //if (existingAnalysis != null)
        //{
        //    return MapToResponse(existingAnalysis);
        //}



        // Extract text based on file type
        var extractedText = await _resumeTextExtractor
    .ExtractTextAsync(
        resume.FilePath,
        resume.FileType
    );


        extractedText = _textCleaner
            .Clean(extractedText);


        // AI Analysis
        var analysis = await _resumeAnalyzer
            .AnalyzeAsync(extractedText);



        analysis.Id = Guid.NewGuid();

        analysis.ResumeId = resume.Id;

        analysis.ExtractedText = extractedText;

        analysis.CreatedAt = DateTime.UtcNow;




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


            Skills = DeserializeList(analysis.Skills),

            Experience = DeserializeList(analysis.Experience),

            Education = DeserializeList(analysis.Education),

            Projects = DeserializeList(analysis.Projects),

            Certifications = DeserializeList(analysis.Certifications),

            Strengths = DeserializeList(analysis.Strengths),

            Weaknesses = DeserializeList(analysis.Weaknesses),


            ResumeScore = analysis.ResumeScore,

            CreatedAt = analysis.CreatedAt
        };
    }

    private static List<string> DeserializeList(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        return JsonSerializer.Deserialize<List<string>>(json)
               ?? new List<string>();
    }
}