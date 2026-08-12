using System.Text.Json;
using InterviewAce.Application.DTOs.JobMatchAnalysis;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Application.Interfaces.JobMatchAnalysis;
using InterviewAce.Application.Interfaces.Persistence;

using ResumeAnalysisEntity = InterviewAce.Domain.Entities.ResumeAnalysis;
using JobDescriptionEntity = InterviewAce.Domain.Entities.JobDescription;
using JobMatchAnalysisEntity = InterviewAce.Domain.Entities.JobMatchAnalysis;

namespace InterviewAce.Application.Services.JobMatchAnalysis;

public class JobMatchAnalysisService : IJobMatchAnalysisService
{
    private readonly IJobMatchAnalysisRepository _repository;
    private readonly IResumeAnalysisRepository _resumeAnalysisRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IAIProvider _aiProvider;

    public JobMatchAnalysisService(
        IJobMatchAnalysisRepository repository,
        IResumeAnalysisRepository resumeAnalysisRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IAIProvider aiProvider)
    {
        _repository = repository;
        _resumeAnalysisRepository = resumeAnalysisRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _aiProvider = aiProvider;
    }

    public async Task<JobMatchAnalysisResponseDto> AnalyzeAsync(
        Guid userId,
        AnalyzeJobMatchRequestDto request)
    {
        var resumeAnalysis =
            await _resumeAnalysisRepository.GetByIdAsync(
                request.ResumeAnalysisId,
                userId);

        if (resumeAnalysis == null)
        {
            throw new KeyNotFoundException(
                "Resume analysis not found.");
        }

        var jobDescription =
            await _jobDescriptionRepository.GetByIdAndUserIdAsync(
                request.JobDescriptionId,
                userId);

        if (jobDescription == null)
        {
            throw new KeyNotFoundException(
                "Job description not found.");
        }

        var prompt = BuildPrompt(
            resumeAnalysis,
            jobDescription);

        var aiResponse =
            await _aiProvider.GenerateResponseAsync(prompt);

        var result =
            JsonSerializer.Deserialize<JobMatchAnalysisResponseDto>(
                aiResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result == null)
        {
            throw new InvalidOperationException(
                "AI returned an invalid job match analysis response.");
        }

        var analysis = new JobMatchAnalysisEntity
        {
            Id = Guid.NewGuid(),
            ResumeAnalysisId = resumeAnalysis.Id,
            JobDescriptionId = jobDescription.Id,
            MatchScore = Math.Clamp(
                result.MatchScore,
                0,
                100),
            MatchingSkills =
                JsonSerializer.Serialize(result.MatchingSkills),
            MissingSkills =
                JsonSerializer.Serialize(result.MissingSkills),
            ExperienceMatch =
                result.ExperienceMatch ?? string.Empty,
            Strengths =
                JsonSerializer.Serialize(result.Strengths),
            Gaps =
                JsonSerializer.Serialize(result.Gaps),
            Recommendations =
                JsonSerializer.Serialize(result.Recommendations),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(analysis);
        await _repository.SaveChangesAsync();

        return MapToResponse(analysis);
    }

    public async Task<JobMatchAnalysisResponseDto?> GetByIdAsync(
        Guid userId,
        Guid id)
    {
        var analysis =
            await _repository.GetByIdAndUserIdAsync(
                id,
                userId);

        if (analysis == null)
        {
            return null;
        }

        return MapToResponse(analysis);
    }

    public async Task<List<JobMatchAnalysisResponseDto>> GetAllAsync(
        Guid userId)
    {
        var analyses =
            await _repository.GetByUserIdAsync(userId);

        return analyses
            .Select(MapToResponse)
            .ToList();
    }

    private static string BuildPrompt(
        ResumeAnalysisEntity resumeAnalysis,
        JobDescriptionEntity jobDescription)
    {
        return """
        You are an expert technical recruiter and career advisor.

        Analyze how well the candidate's resume matches the provided job description.

        RESUME ANALYSIS:

        Skills:
        """ + resumeAnalysis.Skills + """

        Experience:
        """ + resumeAnalysis.Experience + """

        Education:
        """ + resumeAnalysis.Education + """

        Projects:
        """ + resumeAnalysis.Projects + """

        Certifications:
        """ + resumeAnalysis.Certifications + """

        Strengths:
        """ + resumeAnalysis.Strengths + """

        Weaknesses:
        """ + resumeAnalysis.Weaknesses + """

        Resume Score:
        """ + resumeAnalysis.ResumeScore + """

        JOB DESCRIPTION:

        Title:
        """ + jobDescription.Title + """

        Company:
        """ + jobDescription.CompanyName + """

        Description:
        """ + jobDescription.Description + """

        Return ONLY valid JSON.

        Do not include markdown.
        Do not include ```json.
        Do not include explanations outside the JSON.

        Required JSON structure:

        {
          "matchScore": 0,
          "matchingSkills": [],
          "missingSkills": [],
          "experienceMatch": "",
          "strengths": [],
          "gaps": [],
          "recommendations": []
        }

        Rules:

        - matchScore must be an integer from 0 to 100.
        - matchingSkills must contain skills supported by the resume.
        - missingSkills must contain relevant job skills not sufficiently demonstrated by the resume.
        - experienceMatch should briefly describe how well the candidate's experience matches the role.
        - strengths should identify the strongest areas of alignment.
        - gaps should identify important weaknesses or missing evidence.
        - recommendations should provide practical improvements.
        - Do not invent experience, skills, certifications, or projects that are not supported by the resume analysis.
        """;
    }

    private static JobMatchAnalysisResponseDto MapToResponse(
        JobMatchAnalysisEntity analysis)
    {
        return new JobMatchAnalysisResponseDto
        {
            Id = analysis.Id,
            ResumeAnalysisId = analysis.ResumeAnalysisId,
            JobDescriptionId = analysis.JobDescriptionId,
            MatchScore = analysis.MatchScore,
            MatchingSkills =
                DeserializeList(analysis.MatchingSkills),
            MissingSkills =
                DeserializeList(analysis.MissingSkills),
            ExperienceMatch =
                analysis.ExperienceMatch,
            Strengths =
                DeserializeList(analysis.Strengths),
            Gaps =
                DeserializeList(analysis.Gaps),
            Recommendations =
                DeserializeList(analysis.Recommendations),
            CreatedAt = analysis.CreatedAt
        };
    }

    private static List<string> DeserializeList(
        string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json)
                   ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}