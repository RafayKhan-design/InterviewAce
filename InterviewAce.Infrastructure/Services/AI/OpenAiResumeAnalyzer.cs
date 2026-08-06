using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Infrastructure.Services.AI;

public class OpenAiResumeAnalyzer : IResumeAnalyzer
{
    public Task<ResumeAnalysis> AnalyzeAsync(string extractedText)
    {
        // Temporary implementation
        var analysis = new ResumeAnalysis
        {
            ExtractedText = extractedText,

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

        return Task.FromResult(analysis);
    }
}