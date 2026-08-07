using System.Text.Json;
using InterviewAce.Application.DTOs.AI;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Infrastructure.Services.AI;

public class AIResumeAnalyzer : IResumeAnalyzer
{
    private readonly IAIProvider _aiProvider;


    public AIResumeAnalyzer(
        IAIProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }



    public async Task<ResumeAnalysis> AnalyzeAsync(
        string extractedText)
    {
        var prompt = $"""
        Analyze this resume.

        Return ONLY valid JSON.

        Required fields:

        skills
        experience
        education
        projects
        certifications
        strengths
        weaknesses
        resumeScore


        Resume:

        {extractedText}
        """;


        var response = await _aiProvider
            .GenerateResponseAsync(prompt);



        AIResumeAnalysisDto? aiAnalysis = null;


        try
        {
            aiAnalysis = JsonSerializer.Deserialize<AIResumeAnalysisDto>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            // Temporary fallback
        }



        var analysis = new ResumeAnalysis
        {
            ExtractedText = extractedText,

            Skills = aiAnalysis?.Skills,

            Experience = aiAnalysis?.Experience,

            Education = aiAnalysis?.Education,

            Projects = aiAnalysis?.Projects,

            Certifications = aiAnalysis?.Certifications,

            Strengths = aiAnalysis?.Strengths,

            Weaknesses = aiAnalysis?.Weaknesses,

            ResumeScore = aiAnalysis?.ResumeScore ?? 0,

            CreatedAt = DateTime.UtcNow
        };


        return analysis;
    }
}