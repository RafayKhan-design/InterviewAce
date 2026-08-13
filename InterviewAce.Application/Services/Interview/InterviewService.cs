using System.Text.Json;
using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Application.Interfaces.Interview;
using InterviewAce.Application.Interfaces.Persistence;

using InterviewEntity =
    InterviewAce.Domain.Entities.Interview;

using InterviewQuestionEntity =
    InterviewAce.Domain.Entities.InterviewQuestion;

using ResumeAnalysisEntity =
    InterviewAce.Domain.Entities.ResumeAnalysis;

using JobDescriptionEntity =
    InterviewAce.Domain.Entities.JobDescription;

using InterviewAIResponse =
    InterviewAce.Application.DTOs.Interview.InterviewAIResponse;

namespace InterviewAce.Application.Services.Interview;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repository;
    private readonly IResumeAnalysisRepository _resumeAnalysisRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IAIProvider _aiProvider;

    public InterviewService(
        IInterviewRepository repository,
        IResumeAnalysisRepository resumeAnalysisRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IAIProvider aiProvider)
    {
        _repository = repository;
        _resumeAnalysisRepository = resumeAnalysisRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _aiProvider = aiProvider;
    }

    public async Task<GenerateInterviewResponseDto> GenerateAsync(
        Guid userId,
        GenerateInterviewRequestDto request)
    {
        if (request.QuestionCount < 1 || request.QuestionCount > 50)
        {
            throw new ArgumentException(
                "Question count must be between 1 and 50.");
        }

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
            jobDescription,
            request);

        var aiResponse =
            await _aiProvider.GenerateResponseAsync(prompt);

        var cleanedAiResponse = CleanJsonResponse(aiResponse);

        var parsedResponse =
            JsonSerializer.Deserialize<InterviewAIResponse>(
                cleanedAiResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (parsedResponse == null)
        {
            throw new InvalidOperationException(
                "AI returned an invalid interview response.");
        }

        if (parsedResponse.Questions == null ||
            parsedResponse.Questions.Count == 0)
        {
            throw new InvalidOperationException(
                "AI did not generate any interview questions.");
        }

        var questions = parsedResponse.Questions
            .Take(request.QuestionCount)
            .Select((question, index) =>
                new InterviewQuestionEntity
                {
                    Id = Guid.NewGuid(),
                    Order = index + 1,
                    Question = question.Question?.Trim()
                        ?? string.Empty,
                    QuestionType =
                        string.IsNullOrWhiteSpace(
                            question.QuestionType)
                            ? request.InterviewType
                            : question.QuestionType.Trim(),
                    ExpectedTopics =
                        JsonSerializer.Serialize(
                            question.ExpectedTopics ?? new List<string>()),
                    Difficulty =
                        string.IsNullOrWhiteSpace(
                            question.Difficulty)
                            ? request.Difficulty
                            : question.Difficulty.Trim(),
                    CreatedAt = DateTime.UtcNow
                })
            .ToList();

        if (questions.Count == 0)
        {
            throw new InvalidOperationException(
                "No valid interview questions were generated.");
        }

        var interview = new InterviewEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ResumeAnalysisId = resumeAnalysis.Id,
            JobDescriptionId = jobDescription.Id,
            Title = string.IsNullOrWhiteSpace(parsedResponse.Title)
                ? $"{jobDescription.Title} Interview"
                : parsedResponse.Title.Trim(),
            InterviewType = request.InterviewType,
            Difficulty = request.Difficulty,
            QuestionCount = questions.Count,
            CreatedAt = DateTime.UtcNow,
            Questions = questions
        };

        await _repository.AddAsync(interview);
        await _repository.SaveChangesAsync();

        return MapToResponse(interview);
    }

    public async Task<GenerateInterviewResponseDto?> GetByIdAsync(
        Guid userId,
        Guid interviewId)
    {
        var interview =
            await _repository.GetByIdAndUserIdAsync(
                interviewId,
                userId);

        if (interview == null)
        {
            return null;
        }

        return MapToResponse(interview);
    }

    public async Task<List<GenerateInterviewResponseDto>> GetAllAsync(
        Guid userId)
    {
        var interviews =
            await _repository.GetByUserIdAsync(userId);

        return interviews
            .Select(MapToResponse)
            .ToList();
    }

    private static string CleanJsonResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            throw new InvalidOperationException(
                "AI returned an empty response.");
        }

        var cleaned = response.Trim();

        if (cleaned.StartsWith("```json",
            StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned.Substring(7);
        }
        else if (cleaned.StartsWith("```",
            StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned.Substring(3);
        }

        if (cleaned.EndsWith("```"))
        {
            cleaned = cleaned.Substring(
                0,
                cleaned.Length - 3);
        }

        return cleaned.Trim();
    }

    private static string BuildPrompt(
    ResumeAnalysisEntity resumeAnalysis,
    JobDescriptionEntity jobDescription,
    GenerateInterviewRequestDto request)
    {
        return """
    You are an experienced human interviewer conducting a real interview.
    You have read this candidate's resume and the job description
    beforehand, the way any interviewer would prep, and you're generating
    the actual questions you'd ask in the room.

    ╔══════════════════════════════════════════════════════════════════╗
    ║ RULE ZERO — READ FIRST                                            ║
    ╠══════════════════════════════════════════════════════════════════╣
    ║ Every question must be groundable in the resume analysis and/or   ║
    ║ job description provided below.                                   ║
    ║                                                                    ║
    ║ DO NOT invent a skill, technology, employer, project, or          ║
    ║   achievement the candidate doesn't actually have, just to ask a  ║
    ║   question about it.                                              ║
    ║ DO NOT invent a company practice, tool, or team structure that    ║
    ║   isn't stated in the job description, just to ask about it.      ║
    ║ It's fine to ask general role-relevant questions not tied to a    ║
    ║   specific resume line (e.g., a standard SystemDesign prompt) —   ║
    ║   the rule is: don't fabricate candidate facts, not "every        ║
    ║   question must quote the resume."                                ║
    ╚══════════════════════════════════════════════════════════════════╝

    ==================================================
    CANDIDATE RESUME ANALYSIS
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
    TARGET JOB
    ==================================================
    Title:
    """ + jobDescription.Title + """
    Company:
    """ + jobDescription.CompanyName + """
    Description:
    """ + jobDescription.Description + """

    ==================================================
    INTERVIEW CONFIGURATION (fixed for this entire interview)
    ==================================================
    Interview Type:
    """ + request.InterviewType + """
    Difficulty:
    """ + request.Difficulty + """
    Requested Question Count:
    """ + request.QuestionCount + """

    ==================================================
    1. TYPE × DIFFICULTY MATRIX — FIND YOUR EXACT CELL, USE ONLY THAT ONE
    ==================================================

    Difficulty means something different depending on interview type.
    "Expert" in a Technical interview is not the same thing as "Expert" in
    an HR interview. Locate the row for the CONFIGURED Interview Type
    below, then apply the guidance for the CONFIGURED Difficulty within
    that row. Do not blend guidance across types or apply generic
    "harder = more architecture" logic to non-technical types.

    --- TECHNICAL ---
    Beginner: Fundamental syntax, core concepts, definitions, basic usage
      of languages/frameworks/tools the candidate has actually listed.
      E.g., "what's the difference between X and Y," "when would you use
      Z instead of W."
    Intermediate: Practical, real-world application — how they'd
      implement a feature, debug a common issue, or explain a decision
      they made in one of their actual projects.
    Advanced: Deeper internals, performance trade-offs, debugging
      non-trivial or ambiguous issues, comparing competing approaches
      with reasoning.
    Expert: Senior-level technical decision-making — trade-offs across
      an entire system or codebase, mentoring/technical-leadership
      scenarios, deep specialization probes, handling failure at scale.

    --- BEHAVIORAL ---
    Beginner: Simple, low-stakes "tell me about a time" prompts on
      teamwork, communication, or a straightforward past situation.
    Intermediate: Conflict resolution, prioritization under pressure,
      receiving/giving feedback, moderate ambiguity.
    Advanced: Leading through ambiguity, cross-team conflict, owning a
      real failure or mistake, influencing people without formal
      authority.
    Expert: Organization-level leadership scenarios, navigating
      conflicting stakeholder pressure, managing underperformance or
      difficult people decisions, shaping team culture.
    (Never mention "architecture" or "scalability" for this type — those
    are Technical/SystemDesign concepts.)

    --- HR ---
    Beginner: Motivation for the role, why this company, basic
      strengths/weaknesses, availability and logistics.
    Intermediate: Career goal alignment with the role, handling
      constructive feedback, culture fit, basic compensation
      expectations conversation.
    Advanced: Handling competing offers or counteroffers, negotiation
      approach, ambiguous workplace ethics scenarios, coherence of the
      candidate's overall career narrative.
    Expert: Executive/senior-level expectations, alignment with company
      strategic direction, complex interpersonal or ethical scenarios,
      what they'd change about how the org/team operates.
    (Never mention technical implementation details for this type.)

    --- MIXED ---
    Combine Technical and Behavioral questions using their respective
    rows above at the SAME configured difficulty level. Split the
    requested question count as evenly as possible between the two
    (if the count is odd, give the extra question to Technical). Each
    question keeps its own real sub-type in its metadata (see Section 3)
    — never label an individual question's type as "Mixed."

    --- SYSTEMDESIGN ---
    Beginner: Foundational concepts only — what horizontal vs. vertical
      scaling means, what a load balancer does, basic client-server or
      caching concepts. Do NOT ask for a full system design at this
      level — that's not what "Beginner SystemDesign" means in practice.
      FORBIDDEN PHRASING at this tier — if a question contains any of
      these patterns, it's actually an Intermediate-or-higher design
      task and does not belong here: "describe a basic architecture for
      X and how you would approach designing it," "how would you
      approach designing a system for X," "design a system that..." Use
      concept/consideration framing instead: "what would you need to
      think about when...," "what's the role of X in...," "what's the
      difference between X and Y."
    Intermediate: Design a small, well-scoped system (e.g., a URL
      shortener, a basic notification service) with light trade-off
      discussion.
    Advanced: Design a larger system with real constraints (e.g., a rate
      limiter, a chat system) — discuss trade-offs, bottlenecks, and
      failure handling.
    Expert: Design a large-scale distributed system with multiple hard
      constraints (consistency vs. availability, partition tolerance,
      capacity estimation) and defend the trade-offs under follow-up
      pressure, the way a staff/principal-level interview would.

    EVERY question in a SystemDesign interview — at every difficulty —
    must involve actual system-level elements: architecture, components,
    data flow, scale, reliability/availability, or engineering trade-offs
    between competing designs.

    EXPLICITLY BANNED — these are TECHNICAL questions, not SystemDesign,
    no matter how they're dressed up, and must NEVER appear in a
    SystemDesign interview at any difficulty:
    - Asking why/whether to use one specific tool, library, or technique
      over another with no system context (e.g., "SHAP vs. LIME,"
      "TensorFlow vs. PyTorch," "supervised vs. unsupervised learning,"
      "which framework would you pick and why").
    - Hyperparameter tuning, model pruning, or other model-internals
      optimization questions with no architecture/scale/component
      framing.
    - Any question whose correct answer is a technique choice or ML
      concept explanation rather than a system/component/data-flow
      description.
    If you want to probe related technical depth, reframe it at the
    system level instead (e.g., "how would you build interpretability
    and monitoring into a production ML pipeline serving this model at
    scale" — that's SystemDesign; "walk me through SHAP vs. LIME" or
    "TensorFlow or PyTorch — which and why" is not, regardless of
    difficulty tier).

    ANTI-TEMPLATE RULE: Do not generate multiple questions that are the
    same underlying design task with only the application domain
    swapped (e.g., "design a real-time, low-latency, scalable system for
    [chatbots / recommenders / NLP / computer vision / predictive
    maintenance]" repeated five times is ONE template, not five
    questions — this violates the duplicate-concept rule in Section 4
    even though the wording differs each time). Instead, spread
    questions across genuinely different system-design dimensions, for
    example: data ingestion/pipelines, consistency/CAP trade-offs,
    multi-tenancy or framework/platform integration, cost vs. reliability
    trade-offs, monitoring/observability, security/compliance, model
    versioning and safe rollback, human-in-the-loop or explainability
    requirements at the architecture level, edge vs. cloud trade-offs,
    and incident response/failure recovery. No two questions in the same
    interview should draw from the same dimension.

    ==================================================
    2. SOUND LIKE A HUMAN INTERVIEWER, NOT AN AI
    ==================================================

    These questions will be read aloud or presented as if a real person
    is asking them in the room. Follow these rules:

    - Reference specific resume/job details the way a prepared human
      interviewer would, naturally and in passing — e.g., "You used SHAP
      and LIME on the land cover project — walk me through why you
      picked those two over something like Grad-CAM alone," not "Based
      on your resume analysis, you have experience with explainable AI
      techniques, can you elaborate?"
    - Never reference the resume analysis, job description, or this
      prompt as documents ("according to your resume," "your resume
      analysis shows," "based on the weaknesses identified") — a human
      interviewer doesn't cite a report, they just know things about you
      and ask naturally. This includes naming the resume analysis's own
      field/category labels — never say "in your strengths," "in your
      skills list," "under your weaknesses," or similar. If you want to
      probe a claimed strength, phrase it as something the candidate
      said or showed, not as a citation of a report section: "You said
      communication's a strong suit of yours — give me an example," not
      "You've mentioned strong communication in your strengths."
    - Vary sentence openings. Do not start most questions the same way
      (e.g., don't make every question "Can you describe a time when...
      " or "Could you walk me through..."). Mix direct questions,
      scenario setups, and follow-up-style phrasing.
    - Avoid stiff/corporate-sounding vocabulary: "leverage," "utilize,"
      "delve into," "furthermore," "robust," "synergy," "in today's
      landscape." Use plain, direct interview language instead ("use,"
      "dig into," "also," "solid," "these days").
    - Keep most questions to one clear ask. Don't stack three sub-
      questions into one run-on sentence — that's a written-document
      pattern, not how interviewers actually talk.
    - It's fine — encouraged, even — to phrase some questions the way a
      person would mid-conversation: contractions, a short lead-in
      before the actual question, natural rhythm.

    ==================================================
    3. QUESTION METADATA RULES
    ==================================================

    If the configured Interview Type is NOT "Mixed": every single
    question's "questionType" must equal the configured Interview Type
    exactly (e.g., every question in a SystemDesign interview is typed
    "SystemDesign," even if it has some technical flavor — see the
    SystemDesign content rule in Section 1, which prevents this from
    becoming an excuse to drift into generic Technical questions).

    If the configured Interview Type IS "Mixed": "questionType" must
    reflect the ACTUAL category of that specific question — "Technical"
    or "Behavioral" only (Mixed combines just these two per Section 1).
    Never write "Mixed" as an individual question's questionType.

    "difficulty" must equal the interview's configured Difficulty for
    every question (there is only one difficulty per interview) — but the
    intrinsic complexity of the questions themselves may ramp up slightly
    as "order" increases, so the interview builds naturally rather than
    feeling flat.

    "expectedTopics" should list concrete things a real interviewer would
    be listening for in a strong answer — specific concepts, trade-offs,
    or behaviors — not vague restatements of the question itself.

    DIFFICULTY CALIBRATION SELF-CHECK: For each question, ask — could
    this exact question be asked unchanged at a lower configured
    Difficulty for this type without feeling out of place? If yes, it
    isn't distinctly calibrated to the configured tier — add the
    complexity, stakes, or ambiguity called for by that tier's row in
    Section 1 (e.g., an "Advanced" Behavioral question about handling
    feedback should involve conflicting or high-stakes feedback in an
    ambiguous situation, not a simple "how do you take feedback" prompt
    that would work fine at Beginner or Intermediate too).

    ==================================================
    4. COUNT AND DUPLICATE RULES
    ==================================================

    Generate exactly the requested "Requested Question Count" number of
    questions. Before finalizing, count your questions array and confirm
    it matches exactly — add or trim if it doesn't.

    No two questions may test the same underlying concept/scenario even
    if worded differently — check semantically, not just for exact text
    duplicates, before finalizing.

    ==================================================
    5. TITLE
    ==================================================

    Generate a specific, sensible "title" that reflects the interview
    type, difficulty, and target role — e.g., "Technical Interview —
    AI Engineer (Intermediate)" — not a generic placeholder.

    ==================================================
    6. OUTPUT FORMAT — CRITICAL
    ==================================================

    Your entire response must be a single raw JSON object and nothing
    else. The very first character of your response must be { and the
    very last character must be }. Do not wrap the JSON in a code block,
    do not add a language tag, do not add any fence or delimiter before
    or after it, and do not add any explanation, greeting, or commentary
    outside the JSON. Output the object directly, as if it will be
    passed straight into a JSON parser with no cleanup.

    REQUIRED JSON FORMAT
    {
      "title": "",
      "questions": [
        {
          "order": 1,
          "question": "",
          "questionType": "",
          "expectedTopics": [],
          "difficulty": ""
        }
      ]
    }

    ==================================================
    7. FINAL VALIDATION — RUN BEFORE OUTPUTTING
    ==================================================

    - Every question matches the Type × Difficulty matrix cell for the
      configured Interview Type and Difficulty (Section 1) — no
      technical-style "architecture/scalability" language leaked into
      Behavioral or HR questions, no full system-design asks (or their
      forbidden phrasings) at SystemDesign Beginner level, and —
      importantly — no tool/technique-choice or model-internals
      questions (SHAP vs. LIME, TensorFlow vs. PyTorch, supervised vs.
      unsupervised, hyperparameter tuning, etc.) anywhere in a
      SystemDesign interview at any difficulty.
    - For SystemDesign interviews specifically: no two questions reuse
      the same underlying design template with only the application
      domain swapped (see Anti-Template Rule in Section 1) — confirm
      each question draws from a genuinely different design dimension.
    - No question invents a skill, project, employer, or company detail
      not present in the resume analysis or job description.
    - No question or its metadata references "your resume," "your
      resume analysis," "in your strengths/skills/weaknesses," or any
      other citation of the resume analysis's own field names.
    - For non-Mixed interviews, every questionType exactly equals the
      configured Interview Type. For Mixed interviews, every
      questionType is "Technical" or "Behavioral" (never "Mixed").
    - Each question passes the difficulty calibration self-check —
      distinctly harder/more complex than what the tier below would
      produce, not a generic prompt that would fit any tier.
    - questions.length exactly equals the Requested Question Count.
    - No two questions test the same underlying concept.
    - Every difficulty field equals the configured Difficulty.
    - Valid JSON, no markdown, no code fences, no nulls, no text outside
      the JSON object.
    """;
    }



    private static GenerateInterviewResponseDto MapToResponse(
        InterviewEntity interview)
    {
        return new GenerateInterviewResponseDto
        {
            Id = interview.Id,
            ResumeAnalysisId = interview.ResumeAnalysisId,
            JobDescriptionId = interview.JobDescriptionId,
            Title = interview.Title,
            InterviewType = interview.InterviewType,
            Difficulty = interview.Difficulty,
            QuestionCount = interview.QuestionCount,
            Questions = interview.Questions
                .OrderBy(x => x.Order)
                .Select(x => new InterviewQuestionDto
                {
                    Id = x.Id,
                    Order = x.Order,
                    Question = x.Question,
                    QuestionType = x.QuestionType,
                    ExpectedTopics =
                        DeserializeList(x.ExpectedTopics),
                    Difficulty = x.Difficulty
                })
                .ToList(),
            CreatedAt = interview.CreatedAt
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