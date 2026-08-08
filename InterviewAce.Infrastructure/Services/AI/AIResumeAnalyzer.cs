using System.Text.Json;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Domain.Entities;
using InterviewAce.Infrastructure.Services.AI.Models;

namespace InterviewAce.Infrastructure.Services.AI;

public class AIResumeAnalyzer : IResumeAnalyzer
{
    private readonly IAIProvider _aiProvider;

    public AIResumeAnalyzer(IAIProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    public async Task<ResumeAnalysis> AnalyzeAsync(string extractedText)
    {
        var prompt = $$"""
You are an expert technical recruiter, ATS specialist, and professional resume consultant.

Your task is to analyze the resume provided below.

Your analysis MUST be based ONLY on information explicitly present in the resume.

DO NOT hallucinate.
DO NOT invent employment.
DO NOT invent technologies.
DO NOT invent degrees.
DO NOT invent certifications.
DO NOT assume that a project means professional employment.

Return ONLY valid JSON.
Do NOT return markdown.
Do NOT return ```json.
Do NOT include explanations outside the JSON.

==================================================
OUTPUT FORMAT
==================================================

Return exactly this JSON structure:

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

Every field MUST be an array except resumeScore.

resumeScore MUST be an integer from 0 to 100.

Never return null.

==================================================
1. SKILLS
==================================================

Extract technologies, programming languages, frameworks, databases,
architectures, development tools, platforms, methodologies, and technical
concepts explicitly mentioned in the resume.

Examples:

C#
ASP.NET Core
Java
SQL
PostgreSQL
REST APIs
Docker
Git
Object-Oriented Programming

Do NOT create skills that are not explicitly mentioned.

Avoid unnecessary duplication.

==================================================
2. EXPERIENCE
==================================================

This is extremely important.

Only include actual professional experience.

Valid evidence includes:

- Company/employer
- Job title connected to an employer
- Internship
- Employment dates
- Freelance work
- Contract work
- Professional responsibilities
- Professional clients

For each experience item, summarize the role in a concise string.

Example:

"Junior Software Developer at ABC Technologies — developed ASP.NET Core APIs and worked with SQL Server."

If the resume only contains projects and education, return [].

IMPORTANT:

Do NOT treat these as employment evidence:

- "Software Developer" appearing only as a resume heading
- Professional summary
- Career objective
- Skills
- Project descriptions
- Personal interests

A title alone does NOT prove employment.

==================================================
3. EDUCATION
==================================================

Extract ONLY formal academic degrees and academic programs.

The resume section where an item appears is important.

An item is EDUCATION only when it represents formal academic study
toward an academic qualification.

Examples of valid education:

- BS Computer Science
- Bachelor of Science
- Bachelor of Computer Science
- MS Computer Science
- Master of Science
- PhD Computer Science
- Intermediate / ICS
- A-Levels
- Matriculation
- High School

Examples:

"BS Computer Science (2023-2027) — National University of Modern Languages"

"ICS Physics (2021-2022) — Punjab Group of Colleges"

"Matriculation (2019-2020) — M.N Education Campus"

IMPORTANT:

A university name alone does NOT make something education.

The item must represent an academic degree, academic program,
or formal school/college education.

==================================================
EDUCATION EXCLUSION RULE
==================================================

NEVER classify the following as education:

- Certifications
- Certificates
- Micro-certifications
- Online courses
- Professional courses
- Training
- Workshops
- Bootcamps
- Skill certificates
- CS50
- Kaggle certificates
- Microsoft certifications
- AWS certifications
- Google certifications
- Coursera certificates
- Udemy certificates

For example:

"Harvard University — CS50: Introduction to Computer Science"

is NOT education when presented as a certification/course.

It belongs under certifications.

==================================================
4. CERTIFICATIONS
==================================================

Extract certifications, certificates, courses, and professional credentials.

Examples:

"Microsoft — Foundational C# Certification"

"Kaggle — Feature Engineering Micro-Certification"

"Harvard University — CS50: Introduction to Computer Science"

IMPORTANT:

If an item contains words such as:

- Certification
- Certificate
- Micro-Certification
- Course
- Professional Certificate
- Credential

classify it as CERTIFICATION, not education.

==================================================
CRITICAL CLASSIFICATION RULE
==================================================

Each item MUST belong to only ONE category.

Never place the same item in both education and certifications.

If an item could appear to belong to both categories, determine what
the item actually represents.

Examples:

"BS Computer Science — National University"
→ education

"Harvard CS50 — Introduction to Computer Science"
→ certification/course

"Microsoft Foundational C# Certification"
→ certification

"Kaggle Feature Engineering Micro-Certification"
→ certification

DO NOT duplicate the same item across categories.

==================================================
5. PROJECTS
==================================================

Extract explicitly named projects.

For each project provide a concise description including technologies
when those technologies are explicitly mentioned.

Example:

"Task Manager — ASP.NET Core MVC application using 3-layer architecture and MySQL for CRUD operations."

Do NOT invent project details.

Do NOT convert professional jobs into projects.

==================================================
6. STRENGTHS
==================================================

Identify strengths supported by actual evidence in the resume.

Good examples:

"Strong backend development foundation demonstrated through ASP.NET Core projects."

"Strong OOP knowledge demonstrated through Java and C# projects."

"Hands-on database experience with SQL and MySQL."

Avoid generic praise unless supported by the resume.

Do NOT invent personality traits.

==================================================
7. WEAKNESSES
==================================================

Identify genuine weaknesses in the resume that could reduce its
effectiveness for recruiters or ATS systems.

Weaknesses MUST be based on evidence or clear absence of important
resume information.

Do NOT invent personal weaknesses.

Do NOT criticize the candidate's personality.

Do NOT make assumptions about abilities that are not mentioned.

Focus on resume and career-presentation weaknesses.

Check for the following:

1. Professional experience

If no professional experience is listed:

"Professional experience is limited or not clearly demonstrated."

If professional experience exists, do NOT claim that experience is missing.

2. Quantifiable achievements

Check whether projects or professional roles contain measurable results.

Examples of useful metrics:

- accuracy
- performance improvement
- percentage improvement
- number of users
- response time
- dataset size
- revenue
- cost reduction
- project scale
- processing speed

If meaningful metrics are absent:

"Projects or work experience lack measurable achievements or outcomes."

Only include this when the resume genuinely lacks meaningful
measurable results.

3. Project descriptions

Check whether projects explain:

- what was built
- technologies used
- candidate's contribution
- results or outcomes

If projects contain technical details but lack measurable outcomes,
use a specific weakness such as:

"Project descriptions explain the technologies and features used,
but generally lack measurable outcomes such as performance improvements,
user counts, or other quantifiable results."

Do NOT use this weakness if the resume already contains strong
measurable project outcomes.

4. Technical depth

Check whether technologies are listed without demonstrating how they
were used.

If applicable:

"Several technical skills are listed, but practical usage is not
demonstrated for all of them."

5. Resume completeness

Check for missing sections such as:

- professional experience
- certifications
- projects
- achievements
- GitHub
- LinkedIn
- portfolio

Only mention a missing section if it would reasonably strengthen the
candidate's profile.

6. ATS optimization

Check for:

- unclear section structure
- excessive keywords
- missing job-specific terminology
- lack of measurable achievements
- unclear job titles

7. Career positioning

Check whether the resume clearly targets a specific role.

If the resume contains many unrelated technologies without a clear
career direction:

"Technical skills span multiple areas, but the resume could communicate
a more focused target role."

==================================================
WEAKNESS QUALITY RULES
==================================================

Every weakness must be:

- Specific
- Evidence-based
- Relevant to recruiters
- Actionable
- Concise

Avoid generic statements such as:

"Needs improvement."

"Resume could be better."

"More experience is needed."

"Candidate should improve skills."

Instead explain WHAT is missing and WHY it matters.

==================================================
IMPORTANT
==================================================

Do not generate weaknesses simply to fill the array.

If the resume is strong in an area, do not list that area as a weakness.

Usually return between 2 and 5 meaningful weaknesses.

Never return more than 5 weaknesses.

==================================================
8. RESUME SCORE
==================================================

Calculate a realistic ATS/resume quality score from 0 to 100.

Evaluate:

- Resume structure
- Skills relevance
- Professional experience
- Education
- Projects
- Certifications
- Technical depth
- Achievement evidence
- Quantifiable results
- ATS-friendly content
- Clarity
- Completeness

Scoring guidance:

90-100:
Excellent professional resume with strong experience, measurable
achievements, relevant skills, and excellent structure.

80-89:
Strong resume with good technical content and only minor weaknesses.

70-79:
Good foundation but noticeable gaps such as limited experience,
missing metrics, or incomplete sections.

60-69:
Average resume with several important weaknesses.

50-59:
Weak resume with major missing information.

Below 50:
Very incomplete or poorly structured resume.

IMPORTANT:

Do not automatically give 70.

The score MUST reflect the actual quality of the resume.

==================================================
FINAL VALIDATION
==================================================

Before returning the response, verify:

1. Valid JSON
2. No markdown
3. No null values
4. skills is an array
5. experience is an array
6. education is an array
7. projects is an array
8. certifications is an array
9. strengths is an array
10. weaknesses is an array
11. resumeScore is an integer
12. No hallucinated information
13. No project converted into employment
14. No certification converted into education

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

        // Normalize AI-generated arrays before saving.
        var skills = NormalizeList(aiResult.Skills);
        var experience = NormalizeList(aiResult.Experience);
        var education = NormalizeList(aiResult.Education);
        var projects = NormalizeList(aiResult.Projects);
        var certifications = NormalizeList(aiResult.Certifications);
        var strengths = NormalizeList(aiResult.Strengths);
        var weaknesses = NormalizeList(aiResult.Weaknesses);

        var analysis = new ResumeAnalysis
        {
            Id = Guid.NewGuid(),

            ExtractedText = extractedText,

            Skills = JsonSerializer.Serialize(skills),

            Experience = JsonSerializer.Serialize(experience),

            Education = JsonSerializer.Serialize(education),

            Projects = JsonSerializer.Serialize(projects),

            Certifications = JsonSerializer.Serialize(certifications),

            Strengths = JsonSerializer.Serialize(strengths),

            Weaknesses = JsonSerializer.Serialize(weaknesses),

            ResumeScore = Math.Clamp(
                aiResult.ResumeScore,
                0,
                100),

            CreatedAt = DateTime.UtcNow
        };

        return analysis;
    }

    private static List<string> NormalizeList(
        IEnumerable<string>? items)
    {
        return (items ?? Enumerable.Empty<string>())
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string CleanJsonResponse(string response)
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