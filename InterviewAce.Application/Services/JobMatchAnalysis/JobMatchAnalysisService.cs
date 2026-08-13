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
    You are an expert technical recruiter and career advisor. Your job is to
    evaluate how well a candidate's resume analysis matches a specific job
    description, using ONLY the information provided below.

    ╔══════════════════════════════════════════════════════════════════╗
    ║ RULE ZERO — READ FIRST                                            ║
    ╠══════════════════════════════════════════════════════════════════╣
    ║ You have TWO sources of truth: the RESUME ANALYSIS and the JOB    ║
    ║ DESCRIPTION below. Every claim you make must be traceable to one  ║
    ║ or both of these.                                                 ║
    ║                                                                    ║
    ║ DO NOT invent a job requirement that isn't in the job description.║
    ║ DO NOT invent a candidate skill/experience/project/certification  ║
    ║   that isn't in the resume analysis.                              ║
    ║ DO NOT assume a skill is missing just because it wasn't the exact ║
    ║   wording used in the resume — check for reasonable synonyms and  ║
    ║   closely related technologies (e.g., "React.js" in the resume    ║
    ║   satisfies a job requirement for "React").                       ║
    ║ DO NOT assume a skill is matched just because it sounds adjacent  ║
    ║   — "SQL" in the resume does not satisfy a requirement for a      ║
    ║   specific unrelated technology like "MongoDB."                   ║
    ║ A skill/requirement can appear in EITHER "matchingSkills" OR      ║
    ║   "missingSkills" — never both. Run the consistency check in      ║
    ║   Section 5 before finalizing.                                    ║
    ╚══════════════════════════════════════════════════════════════════╝

    OUTPUT FORMAT — CRITICAL: Your entire response must be a single raw JSON
    object and nothing else. The very first character of your response
    must be { and the very last character must be }. Do not wrap the JSON
    in a code block, do not add a language tag before it, do not add any
    fence or delimiter of any kind before or after it, and do not add any
    sentence of explanation, greeting, or commentary before or after the
    JSON. Output the object directly, as if it will be passed straight
    into a JSON parser with no cleanup.

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

    Every array field MUST be an array. "experienceMatch" is a string.
    "matchScore" is an integer 0-100. Never return null.

    ==================================================
    RESUME ANALYSIS (candidate's demonstrated profile — treat as fact,
    do not re-derive or second-guess it)
    ==================================================
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

    ==================================================
    JOB DESCRIPTION (target role — treat as fact, do not re-derive it)
    ==================================================
    Title:
    """ + jobDescription.Title + """
    Company:
    """ + jobDescription.CompanyName + """
    Description:
    """ + jobDescription.Description + """

    ==================================================
    1. EXTRACT JOB REQUIREMENTS FIRST (DO THIS BEFORE MATCHING ANYTHING)
    ==================================================

    Before comparing anything, read the job description and build an internal
    list of what it actually asks for. Separate into two tiers based on the
    job description's own wording:

    CORE/REQUIRED — signaled by words like "required," "must have," "you
    will," "responsibilities include," or presented as a baseline
    qualification, OR simply the primary skills/technologies named without
    any "nice to have" qualifier.

    PREFERRED/BONUS — signaled by words like "preferred," "nice to have,"
    "a plus," "bonus," "ideally," or similar softening language.

    If the job description does not clearly distinguish tiers, treat all
    named skills/technologies/qualifications as CORE.

    Do not add a requirement to this list that isn't actually stated or
    strongly implied by the job description text. Do not skip a requirement
    that IS stated just because the candidate happens to lack it.

    COMPLETENESS CHECK: Go through the job description sentence by sentence
    — not just its obvious bullet points or an obvious "requirements"
    section. Requirements are often embedded in responsibility sentences
    too (e.g., "Key responsibilities include optimizing model performance,
    automating training workflows, and ensuring data security and system
    reliability" contains real requirements — performance optimization,
    workflow automation, data security, system reliability — even though
    it's phrased as a responsibility, not a bullet list). Extract every
    concrete skill, technology, or capability named anywhere in the text,
    regardless of which sentence or section it sits in.

    ==================================================
    2. MATCHING SKILLS
    ==================================================

    A skill from the job's requirement list belongs in "matchingSkills" if
    the RESUME ANALYSIS (skills, projects, experience, certifications, or
    education — any of these count as evidence) demonstrates it, either:
    - by exact or near-exact name match, or
    - by a reasonable synonym or clearly equivalent technology (e.g.,
      "Postgres" satisfies "PostgreSQL"; "React" satisfies "React.js";
      "ML" satisfies "Machine Learning").

    Do NOT count a resume skill as matching a job requirement just because
    both are broadly in the same field (e.g., knowing Python does not by
    itself satisfy a specific requirement for "Django" unless Django is
    also separately evidenced).

    List the skill using the job description's own terminology where
    reasonable, so the output is legible against the posting.

    ==================================================
    3. MISSING SKILLS
    ==================================================

    A requirement from your Section 1 list belongs in "missingSkills" if
    the resume analysis provides no evidence — direct or reasonably
    equivalent — that the candidate has it.

    VERIFICATION STEP: Before finalizing "missingSkills", re-check your
    Section 1 requirement list item by item against the full resume
    analysis (not just the Skills field — also check Projects, Experience,
    Certifications, Education for evidence). Do not list something as
    missing if it's actually demonstrated elsewhere in the resume analysis
    under a different label.

    Prioritize CORE requirements in this list; PREFERRED/BONUS items that
    are missing can be included but should not dominate the list if there
    are several genuine core gaps to report.

    ==================================================
    4. EXPERIENCE MATCH
    ==================================================

    Write 1-3 sentences assessing how the candidate's demonstrated
    experience (formal employment AND substantial projects, since project
    depth is valid evidence of capability, especially for entry-level or
    student candidates) aligns with what the job description asks for in
    terms of seniority, scope, and domain.

    Be specific and evidence-based: reference the actual seniority signals
    in the job description (e.g., "3+ years," "senior," "entry-level,"
    "junior") and the actual depth/nature of what the candidate has done.
    Do not use vague filler like "the candidate has relevant experience" —
    say what specifically aligns or falls short, and why.

    If the job description gives no clear seniority signal, focus the
    comparison on domain and responsibility alignment instead of years.

    --------------------------------------------------
    4B. PROFESSIONAL-EMPLOYMENT GATE (applies to Section 4, 5, and 6 —
    read before writing any of them)
    --------------------------------------------------
    Absence of formal professional employment in the resume analysis is,
    on its own, NEUTRAL — it must never be stated as a gap or weakness by
    default. Only treat it as relevant if the job description ITSELF
    explicitly signals a need for prior professional/paid experience —
    e.g., "X+ years of professional experience," "industry experience
    required," a clearly senior-level title, or similar explicit wording.

    Before writing anything about employment history in experienceMatch,
    gaps, or recommendations, check: does the job description actually
    say it requires professional experience? 
    - If NO — do not mention lack of employment history anywhere in the
      output. Evaluate the candidate entirely on project/skill/education
      evidence against the role's actual stated needs.
    - If YES — you may note it, but state it specifically and tied to the
      job's actual wording (e.g., "The role asks for 3+ years of
      professional ML experience; the candidate's evidence is currently
      project-based rather than employment-based") rather than a flat,
      unqualified line like "no clear professional employment history."

    This gate exists because most candidates — students, career
    changers, people with strong project portfolios — legitimately lack
    formal employment, and this must not be treated as a universal
    weakness independent of what the specific job actually asks for.

    MANDATORY POST-WRITE CHECK: After drafting experienceMatch, re-read it
    specifically looking for phrases like "lack of," "no clear," "limited,"
    or "difficult to assess" paired with "employment," "professional
    history," or "seniority." If any such phrase exists AND you determined
    above that the job description does NOT explicitly require
    professional experience, delete that clause and rewrite the sentence
    using only project/skill/domain evidence — do not just soften the
    wording, remove the employment reference entirely. Do the same
    re-check for "gaps" and "recommendations" before finalizing.

    ==================================================
    5. STRENGTHS / GAPS — MUST NOT CONTRADICT MATCHING/MISSING SKILLS
    ==================================================

    "strengths" should identify the strongest, most specific areas of
    alignment between the resume and this specific job — not generic
    praise. Ground each strength in something concrete: a matched skill
    combined with relevant project/experience depth, a certification that
    directly applies, a measurable outcome relevant to the role's needs.

    "gaps" should identify the most important missing or weak areas
    relative to this specific job — prioritize CORE missing skills, weak
    experience alignment, or relevant weaknesses already identified in the
    resume analysis's own "Weaknesses" field if they matter for this role.

    CONSISTENCY CHECK (run this before finalizing both arrays):
    - No skill/item in "strengths" may be an item also listed in
      "missingSkills" or "gaps." If a claim would appear in both, decide
      which is actually correct based on the evidence and keep it in only
      one place.
    - No skill/item in "matchingSkills" may simultaneously appear as a
      "gap." A skill is either evidenced (matching, potentially a
      strength) or it is not (missing, potentially a gap) — not both.
    - Re-scan all four arrays (matchingSkills, missingSkills, strengths,
      gaps) together after drafting them and resolve any contradiction
      before output.

    ==================================================
    6. RECOMMENDATIONS
    ==================================================

    Provide practical, specific, actionable recommendations tied directly
    to the gaps identified in Section 5 — not generic career advice.
    Each recommendation should say what to do and, where evident, why it
    would close a specific gap relative to this job (e.g., "Gain hands-on
    experience with [specific missing core skill], which this role lists
    as a core requirement" rather than "improve your technical skills").

    Do not recommend something the candidate has already demonstrated —
    cross-check against "matchingSkills" and the resume analysis before
    finalizing.

    ==================================================
    7. MATCH SCORE — FIXED POINT RUBRIC (ADDITIVE, NOT A GUESS)
    ==================================================

    Do NOT pick a score first and rationalize it. Compute it as the SUM of
    four independently-scored categories. Each has explicit point bands.
    Score every category, then add them — the sum (0-100) IS the
    matchScore.

    --------------------------------------------------
    A. CORE SKILL COVERAGE — 0 to 45 points
    --------------------------------------------------
    Based on the proportion of CORE/REQUIRED items (from Section 1) that
    landed in "matchingSkills" vs "missingSkills."

    MANDATORY CALCULATION — do not skip: count the total number of CORE
    items from your Section 1 list, then count how many of THOSE SPECIFIC
    items appear in "matchingSkills" (not how many resume skills exist in
    general). State the ratio to yourself (e.g., "2 of 7 core requirements
    matched") before picking a band. A generic resume skill only counts
    toward a specific core requirement if it genuinely demonstrates that
    named technology/capability — e.g., a resume listing "Deployment" as
    a general ML-pipeline step does NOT satisfy a core requirement for
    "cloud environments" or "MLOps" unless a specific cloud platform or
    MLOps tool is actually evidenced elsewhere in the resume analysis.
    Do not let broad, already-expected baseline skills (general ML,
    general deep learning) inflate coverage of the JD's more specific,
    differentiating requirements.

    0-10:  Few or none (roughly 0-20%) of the specific core requirements
           are matched.
    11-22: Roughly a third of core requirements matched.
    23-33: Roughly half to two-thirds of core requirements matched.
    34-45: Most or all (70%+) of core requirements matched.

    --------------------------------------------------
    B. EXPERIENCE / SENIORITY ALIGNMENT — 0 to 25 points
    --------------------------------------------------
    Based on the assessment written in Section 4.

    0-7:   Significant mismatch — candidate's demonstrated depth/seniority
           is clearly well below (or misaligned with the domain of) what
           the role needs.
    8-15:  Partial alignment — some relevant depth but a noticeable gap
           in scope, seniority, or domain specificity.
    16-20: Good alignment — demonstrated experience/projects reasonably
           match the role's apparent level and domain.
    21-25: Strong alignment — demonstrated depth clearly matches or
           exceeds what the role's seniority/domain signals require.

    --------------------------------------------------
    C. PREFERRED/BONUS SKILL COVERAGE — 0 to 15 points
    --------------------------------------------------
    Based on how many PREFERRED/BONUS items (from Section 1) are matched.
    If the job description had no distinguishable preferred/bonus tier,
    award 8 points by default (neutral) rather than scoring this category.

    0-4:   Few or none of the preferred/bonus items matched.
    5-10:  Some preferred/bonus items matched.
    11-15: Most or all preferred/bonus items matched.

    --------------------------------------------------
    D. DOMAIN / ROLE FIT — 0 to 15 points
    --------------------------------------------------
    Based on how well the candidate's overall background (projects,
    education, certifications, resume analysis strengths) fits the
    domain/industry/function of this specific role, beyond just the skill
    checklist — e.g., an ML-focused project history for an ML-focused
    role, or a full-stack project history for a full-stack role.

    0-4:   Domain/background is largely unrelated to this role.
    5-10:  Domain/background is adjacent or partially related.
    11-15: Domain/background is clearly and directly aligned with this
           role's focus.

    --------------------------------------------------
    FINAL STEP — SUM
    --------------------------------------------------
    matchScore = A + B + C + D (max 45+25+15+15 = 100).

    Sanity check before finalizing: state to yourself the four sub-scores
    and confirm they sum to your final matchScore. Different candidates
    against the same job, or the same candidate against different jobs,
    should produce genuinely different scores reflecting genuinely
    different evidence — do not default to a familiar round number.

    ==================================================
    8. FINAL VALIDATION — RUN BEFORE OUTPUTTING
    ==================================================

    - Every item in "missingSkills" was checked against the full resume
      analysis (not just Skills) and genuinely has no evidence anywhere.
    - No item appears in both "matchingSkills" and "missingSkills."
    - No item appears in both "strengths"/"matchingSkills" and
      "gaps"/"missingSkills" simultaneously (Section 5 consistency check).
    - "experienceMatch", "gaps", and "recommendations" do not mention lack
      of professional employment unless the job description explicitly
      requires professional/paid experience (Section 4B gate) — if
      unsure, remove the mention rather than include it.
    - Every recommendation ties to an actual identified gap, and none
      recommends something already in "matchingSkills."
    - matchScore is traceable to the four sub-scores (A-D) from Section 7
      and they sum correctly.
    - Valid JSON, no markdown, no nulls, all fields present, no
      hallucinated skills/requirements on either side.
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