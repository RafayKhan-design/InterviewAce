using System.Text.Json;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Domain.Entities;
using InterviewAce.Infrastructure.Services.AI.Models;

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
        var prompt = $$"""
You are an expert ATS resume analyzer and professional career consultant.

Analyze the resume carefully and return structured JSON data.

STRICT RULES:

1. Return ONLY valid JSON.
2. Do not include markdown.
3. Do not include explanations.
4. Do not guess information.
5. Never create fake experience, education, skills, projects, or certifications.
6. JSON TYPE RULES:

You MUST strictly follow these data types:

skills: array of strings []
experience: array of strings []
education: array of strings []
projects: array of strings []
certifications: array of strings []
strengths: array of strings []
weaknesses: array of strings []

Never return strings where arrays are expected.

Never return null.
Never return objects.

IMPORTANT:
Every field MUST be an array.
Never return strings.

CLASSIFICATION RULES:

EDUCATION:
Include ONLY formal education.

Allowed:
- Bachelor's degree
- Master's degree
- PhD
- University education
- College education
- School education

Do NOT include:
- Certifications
- Online courses
- Bootcamps
- Training
- Workshops

EDUCATION VALIDATION:

Before adding education, ask:

"Is this an academic degree?"

Accept:
- BS Computer Science
- Bachelor of Science
- MS Computer Science
- Master Degree
- PhD
- University degree

Reject:
- CS50
- Coursera
- Udemy
- Kaggle
- Microsoft Learn
- Bootcamps
- Workshops
- Training

CERTIFICATIONS:
Include:
- Professional certifications
- Online certificates
- Training certificates

Examples:
- Microsoft Certification
- Harvard CS50
- AWS Certification
- Google Certification
- Coursera Certificate
- Kaggle Certificate

SKILLS:
Extract only skills explicitly mentioned.

PROJECTS:
Extract only projects explicitly mentioned.
Do not convert job roles or descriptions into projects.

EXPERIENCE:
Include only:
- Jobs
- Internships
- Freelance work
- Professional roles

Never convert:
- Projects into experience
- Education into experience

EXPERIENCE VALIDATION:

Do not infer experience from:

- Professional summary
- Skills section
- Project descriptions
- Personal titles

A person's title is not proof of employment.

Only extract experience when employment evidence exists.

STRENGTHS:
Identify strengths supported by resume evidence.

WEAKNESSES:
Only include reasonable improvement areas.
Do not invent negative points.
Return [] if none.

RESUME SCORE:
Calculate realistic ATS score from 0-100.

Scoring:

90-100:
Exceptional professional resume.

75-89:
Strong resume with good skills/projects.

60-74:
Average technical foundation.

Below 60:
Missing important sections.

Do not give everyone a high score.

Return exactly:

{
  "skills": [],
  "experience": [],
  "education": [],
  "projects": [],
  "certifications": [],
  "strengths": [],
  "weaknesses": [],
  "resumeScore": 0
}


Resume:

{{extractedText}}
""";


        var response =
            await _aiProvider.GenerateResponseAsync(prompt);


        response = CleanJsonResponse(response);


        Console.WriteLine("AI RESPONSE:");
        Console.WriteLine(response);



        var aiResult =
            JsonSerializer.Deserialize<ResumeAIResponse>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });



        if (aiResult == null)
        {
            throw new Exception(
                "AI response could not be parsed."
            );
        }



        var analysis = new ResumeAnalysis
        {
            Id = Guid.NewGuid(),

            ExtractedText = extractedText,


            Skills = JsonSerializer.Serialize(
                aiResult.Skills ?? new List<string>()),


            Experience = JsonSerializer.Serialize(
                aiResult.Experience ?? new List<string>()),


            Education = JsonSerializer.Serialize(
                aiResult.Education ?? new List<string>()),


            Projects = JsonSerializer.Serialize(
                aiResult.Projects ?? new List<string>()),


            Certifications = JsonSerializer.Serialize(
                aiResult.Certifications ?? new List<string>()),


            Strengths = JsonSerializer.Serialize(
                aiResult.Strengths ?? new List<string>()),


            Weaknesses = JsonSerializer.Serialize(
                aiResult.Weaknesses ?? new List<string>()),


            ResumeScore = aiResult.ResumeScore,


            CreatedAt = DateTime.UtcNow
        };


        return analysis;
    }



    private static string CleanJsonResponse(
        string response)
    {
        response = response.Trim();


        if (response.StartsWith("```"))
        {
            response = response
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
        }


        return response;
    }
}