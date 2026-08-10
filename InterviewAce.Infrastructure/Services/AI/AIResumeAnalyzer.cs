using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Domain.Entities;
using InterviewAce.Infrastructure.Services.AI.Models;
using iText.Layout.Properties;
using iText.StyledXmlParser.Jsoup.Select;
using System.Reflection;
using System.Text.Json;
using UglyToad.PdfPig;

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
You are an expert resume analyst, ATS specialist, recruiter, and career-document evaluator.

╔══════════════════════════════════════════════════════════════════╗
║ RULE ZERO — READ FIRST, THIS OVERRIDES EVERYTHING BELOW           ║
╠══════════════════════════════════════════════════════════════════╣
║ Classification of an item is determined by the SECTION HEADER IT  ║
║ ACTUALLY APPEARS UNDER in the resume text — never by the          ║
║ prestige, fame, or type of institution/company named inside it.   ║
║                                                                    ║
║ "Harvard", "MIT", "Stanford", "Google", "Microsoft" etc. are NOT  ║
║ signals of category. Only the section header + item wording are.  ║
║                                                                    ║
║ WORKED EXAMPLE (this exact case has been gotten wrong before —    ║
║ do not repeat it):                                                ║
║                                                                    ║
║   Resume text:                                                    ║
║   CERTIFICATIONS                                                  ║
║   ● Microsoft — Foundational C# Certification                     ║
║   ● Kaggle — Feature Engineering Micro-Certification               ║
║   ● Harvard University — CS50: Introduction to Computer Science    ║
║                                                                    ║
║   Correct output:                                                 ║
║   "certifications": [                                             ║
║     "Microsoft — Foundational C# Certification",                  ║
║     "Kaggle — Feature Engineering Micro-Certification",           ║
║     "Harvard University — CS50: Introduction to Computer Science" ║
║   ],                                                               ║
║   "education": []                                                 ║
║                                                                    ║
║   WRONG output (do not do this):                                  ║
║   "education": ["Harvard University — CS50: ..."]                 ║
║                                                                    ║
║   All three items sit under the same CERTIFICATIONS header. All   ║
║   three go to certifications. The word "University" inside one    ║
║   of them changes nothing.                                        ║
╚══════════════════════════════════════════════════════════════════╝

Your task is to analyze the resume provided below and return a structured assessment.

IMPORTANT PRINCIPLE:

You must understand the meaning and context of the resume rather than relying only on exact section names or specific professions.

The resume may belong to ANY profession, industry, career level, or background, including but not limited to:

* Software / IT
* Data / AI
* Engineering
* Finance / Accounting
* Marketing
* Sales / Business Development
* Human Resources
* Administration
* Healthcare
* Education
* Design
* Operations
* Management
* Students / Fresh Graduates
* Skilled trades
* Other professional fields

DO NOT assume the candidate is a technical professional.
DO NOT assume that missing information exists.
DO NOT hallucinate.
DO NOT invent employment, employers, technologies, degrees, certifications, projects, or achievements.
DO NOT infer professional experience merely because a person claims a professional title.

Analyze ONLY information supported by the resume.

Return ONLY valid JSON. No markdown. No ```json fences. No explanations outside the JSON.

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

Every field MUST be an array except resumeScore (integer 0-100). Never return null.

==================================================
1. SEMANTIC UNDERSTANDING — SECTION HEADER MAPPING
==================================================

Identify which section each item physically appears under. Use this mapping, but the KEY RULE is: the header governs the category, not the entity names inside the item.

Experience headers: Experience, Professional Experience, Work History, Employment History, Career History, Career Experience, Professional Background, Employment

Education headers: Education, Academic Background, Academic History, Academic Qualifications, Educational Background

Projects headers: Projects, Selected Projects, Academic Projects, Personal Projects, Portfolio, Selected Work, Relevant Work

Certifications headers: Certifications, Credentials, Professional Credentials, Certificates, Courses, Training, Professional Development

If an item's own header says Certifications/Courses/Training/Credentials, it is a certification — full stop — regardless of what institution, company, or brand name is inside the item text. Only re-classify as education if the item's own text explicitly names a degree/diploma/academic program (see Section 4).

==================================================
2. SKILLS
==================================================

Extract skills explicitly demonstrated or explicitly listed in the resume. May include technical, business, marketing, financial, design, management, administrative, or other professionally relevant competencies.

Only include skills supported by the resume. Do NOT invent skills based on job title or profession stereotypes. Prefer normalized names ("MS Excel" → "Microsoft Excel"). Do not merge distinct skills into one entry unless the resume itself presents them as one item; do not split a single resume-stated item into multiple either. Apply this splitting/merging rule consistently across runs — do not vary granularity between calls on the same input.

VERIFICATION STEP — SKILL COMPLETENESS:
After drafting the "skills" array, re-scan every explicit skills/technical-skills
list in the resume line by line. For each item listed there, confirm it appears
in your "skills" output (either as its own entry or clearly merged into a
combined entry per the rule above). If any explicitly listed skill is missing,
add it back before finalizing. Do not silently drop any item that the resume
itself lists under a skills/technical-skills header.

==================================================
2B. ASPIRATIONAL / IN-PROGRESS SKILLS (CRITICAL DISTINCTION)
==================================================

Resumes sometimes separate skills into tiers using headers or wording such as:

"Currently Learning"
"Learning"
"In Progress"
"Familiar with"
"Exposure to"
"Beginner in"
"Aspiring to learn"

Items under these labels are NOT demonstrated skills. Do NOT place them in
the "skills" array alongside genuinely possessed skills.

RULE: The "skills" array must contain ONLY skills the candidate presents
as currently possessed (explicitly listed under a general Skills/
Technical Skills header, or demonstrated in a project/experience entry).

Items explicitly labeled as "currently learning" or equivalent must be
EXCLUDED from the "skills" array entirely.

VERIFICATION STEP: Before finalizing "skills", check whether the resume
contains a "Currently Learning" (or equivalent) label. If so, confirm
none of those items appear in your "skills" output. If any do, remove
them.

This distinction directly feeds into Section 8 (Weaknesses, Check 5) —
do not lose it by merging the two lists.

==================================================
3. EXPERIENCE
==================================================

Extract ONLY genuine professional/work experience: full-time, part-time, internship, freelance, contract, consulting, client work, apprenticeship, or other clearly identified paid/unpaid professional role.

Required evidence (at least one): employer/company/organization name, job title tied to an employer, employment/internship dates, freelance client, work location tied to a role, or explicit wording like "worked at," "employed by," "interned at," "freelanced for."

NOT evidence of employment, by themselves: resume headline, professional title, profile summary, career objective, phrases like "experienced in," "hands-on experience," "experience building," skills, interests, project descriptions (academic or personal).

"Machine Learning Developer with hands-on experience building ML solutions" is a headline/summary claim, NOT employment evidence — if the resume contains no employer, dates, internship, or client, return "experience": [].

VERIFICATION STEP before finalizing experience: for each candidate item, confirm you can point to an employer/organization name OR explicit employment dates OR explicit "worked at/interned at" wording. If you cannot point to one of these, remove the item.

==================================================
4. EDUCATION
==================================================

Extract ONLY formal academic study toward a recognized academic qualification: Bachelor's, Master's, PhD, Associate degree, Diploma, Intermediate, A-Levels, O-Levels, Matriculation, High school, or other formal academic program — evidenced by the item's own wording (degree name, "B.S. in...", "Matriculation," etc.), not by the section header alone and never by institution prestige alone.

An institution name (however famous) does NOT make an item education. A course, certificate, credential, or training program delivered by a university is still a certification, not education, unless the item text itself states it is a formal degree/diploma.

VERIFICATION STEP before finalizing education: for each candidate item, confirm the item's own text names a degree, diploma, or formal academic program level (not just an institution or course title). If it only names a course/certificate/program title with no degree-level language, it does NOT belong here — move it to certifications instead.

==================================================
5. CERTIFICATIONS
==================================================

Extract professional certifications, certificates, courses, credentials, and structured training explicitly listed in the resume — including university-branded courses (e.g., "Harvard University — CS50," "Stanford — Machine Learning Certificate") that are not themselves degrees.

Default rule: if an item sits under a Certifications/Courses/Training/Credentials header and its own text does not state a degree/diploma, it goes here — even if a university name is present.

CATEGORY CONFLICT RESOLUTION (apply to every borderline item):
1. What header does this item sit under in the source text?
2. Does the item's own wording state a degree/diploma/formal academic program?
   - YES → education.
   - NO → certifications (regardless of header, regardless of institution name).
3. Never place the same item in both arrays.

==================================================
6. PROJECTS
==================================================

Extract explicitly identified projects or substantial pieces of work (Projects, Selected Projects, Academic/Personal Projects, Portfolio, Selected Work, Relevant Work). Preserve project name, summarize what was done, include tools/technologies/methods and measurable outcomes ONLY when explicitly stated. Do not invent results. Do not convert jobs or education into projects unless the resume explicitly labels the work as a project.

==================================================
7. STRENGTHS
==================================================

Identify genuine, evidence-based strengths reflecting what the resume demonstrates (e.g., "Strong ML foundation demonstrated through multiple projects with measurable outcomes"). Do NOT invent personality traits (hardworking, passionate, motivated, etc.) unless directly evidenced.

CRITICAL RULE: A strength must be corroborated by evidence OUTSIDE the
profile summary/headline itself — i.e., by a project, work experience,
certification, or education entry. Do NOT list a strength that is merely
a restatement of the candidate's own self-description ("strong
communicator," "reliable," "independent," "detail-oriented," "team
player," etc.) unless something elsewhere in the resume (a project
outcome, a leadership role, a specific achievement) actually
demonstrates it.

Self-declared soft-skill claims with no corroborating evidence must NOT
appear in "strengths" — see Section 8, Check 6, for how to handle these
instead.

==================================================
8. WEAKNESSES — MANDATORY TEST-EACH-CHECK PROCEDURE
==================================================

Weaknesses describe gaps in the RESUME/DOCUMENT, never the person's
character, ability, or potential. No minimum count — "weaknesses": []
is valid. Never more than 5.

You MUST run EVERY check below against the actual resume text before
writing the weaknesses array. Do not skip a check just because a
previous resume you analyzed didn't need it. For each check, answer
internally YES (include) or NO (skip) — do not skip the evaluation
itself.

CHECK 1 — EMPLOYMENT EVIDENCE (CONTEXT-GATED — DO NOT DEFAULT TO YES)
This check exists to catch cases where a resume's own framing creates an
expectation of employment history that the content doesn't back up — NOT
to penalize every resume that happens to lack a job.

Absence of an employer, dates, or employment wording is, on its own,
NEUTRAL. Most resumes — students, fresh graduates, career changers,
researchers, people with strong project/education portfolios — legitimately
have no employment section, and that must never automatically become a
weakness.

Only answer YES on this check if BOTH of the following are true:
1. There is no employer, dates, or explicit employment wording anywhere, AND
2. The resume's own framing (job titles used in the headline, phrases like
   "professional software developer," "X years of experience," a summary
   that reads as if written for an experienced-hire role) creates a
   specific, resume-driven expectation of employment history that the rest
   of the document then fails to support — i.e., the resume is presenting
   itself as more senior/employed than its content demonstrates.

If the candidate's framing is consistent with their actual content (e.g.,
a student resume that says "final-year Computer Science student," or a
project-focused headline with no experience claims), answer NO — do not
include any weakness about employment. This is the default outcome for
most resumes; treat it as such.

If included, the wording must be specific to the mismatch, not a blanket
statement, e.g.: "The summary's professional framing is not yet matched
by documented employment, internship, or client work — project and
academic evidence currently carry that weight instead." Never use
"limited experience," "lacks experience," or similar deficiency framing.

Whether this check fires or not, the remaining checks (2-9) below are
where the real, substantive, per-candidate improvement areas should
surface — skill evidence gaps, thin project detail, unsupported claims,
missing metrics, unclear positioning. Weight your effort toward finding
genuine issues there rather than relying on Check 1 as a default finding.

CHECK 2 — MEASURABLE OUTCOMES
Do projects/work/research consistently lack any numbers, metrics, or
measurable results?
YES (consistently absent) → include: "Work and project descriptions
contain limited measurable outcomes, making their impact difficult to
quantify."
If even some entries have real metrics (accuracy %, scale, results),
answer NO — do not include this.

CHECK 3 — PROJECT DETAIL DEPTH
Are any projects listed as name-only, with no description of contribution,
tools, or outcome?
YES → include: "Project entries provide limited information about the
candidate's contribution, implementation, or outcomes."

CHECK 4 — SKILL EVIDENCE
Take the full "skills" array. For each skill, check whether it is
demonstrated in ANY project, experience, certification, or coursework
entry — not merely asserted by being listed. Generic soft-skill labels
("Time Management," "Adaptability," "Multitasking," "Computer Skills,"
"Problem Solving," "Presentation Skills," etc.) count as UNSUPPORTED
unless a specific project, role, or achievement in the resume actually
shows that skill being used — a single unrelated bullet point elsewhere
does not automatically corroborate every soft skill on the list.

Do the count explicitly: state to yourself how many total skills there
are and how many have zero real corroboration. If half or more of the
list is unsupported (this is common for resumes that are mostly a
bullet list of soft-skill buzzwords with one or two experience lines) →
include: "Several listed skills — particularly generic competencies
such as [name 2-3 specific unsupported ones] — are not demonstrated
through any project, role, or achievement in the resume."

CHECK 5 — ASPIRATIONAL / IN-PROGRESS ITEMS
Does the resume label any items as "Currently Learning," "Familiar with,"
"Exposure to," or similar (see Section 2B), and do those items NOT appear
in any project or experience entry?
YES → include: "Some listed competencies (e.g., under 'Currently
Learning') are not yet demonstrated through projects, work, or coursework
in the resume."

CHECK 6 — UNSUPPORTED SELF-DESCRIPTION
Does the profile summary/headline claim soft skills or traits (e.g.,
"strong communicator," "reliable," "detail-oriented," "team player")
with NO corroborating evidence elsewhere (no project, role, or
achievement that actually demonstrates it)?
YES → include: "Some self-described qualities in the summary (e.g.,
communication or teamwork skills) are not corroborated by specific
evidence elsewhere in the resume."
Do not let these self-declared claims appear in "strengths" either —
see Section 7.

CHECK 7 — MISSING RELEVANT SECTION
Is a section absent (projects, certifications, portfolio links, contact
info) that would genuinely and specifically strengthen THIS candidate's
profile for THIS career stage/field — not a generic "add more sections"
comment?

Explicitly consider: if the resume has NO projects AND NO certifications
AND experience (if any) is thin, informal, or lacks detail (no named
organization, no measurable outcome, no specifics beyond a duration),
this is a genuine, common, and significant gap — not a minor one. This
applies regardless of career stage: even non-technical or early-career
resumes benefit from at least one of projects/certifications/detailed
experience to substantiate their claimed skills, and a resume with none
of these should be flagged.

YES → include a specific, targeted statement naming what's missing and
why it matters for this candidate (e.g., "No projects, certifications,
or detailed work history are included to substantiate the listed
skills").
If nothing is missing that's actually relevant to this candidate → NO,
skip.

CHECK 8 — ATS / STRUCTURE
Is there an observable structural problem in the extracted text itself
(inconsistent date formats, unclear job titles, garbled sections)?
YES → include a specific statement naming the exact observable issue.
Do not invent formatting problems you can't point to in the text.

CHECK 9 — CAREER POSITIONING
Do skills/projects/experience span genuinely unrelated fields with no
coherent direction (not just "has many skills" — those can be related)?
YES → include: "Experience and skills span multiple areas, which may
make the candidate's target role less clear to recruiters."

FINAL RULE: Every weakness in your output must trace back to one of
these 9 checks answering YES. If you write a weakness that doesn't map
to a check above, delete it. If a check answers YES but you're not
confident it's genuinely supported by the text, don't include it —
false negatives (missing a real weakness) are better than false
positives (inventing one).

Interpret all checks according to career-stage context: do not penalize
students, fresh graduates, career changers, researchers, or academics for
lacking professional employment (Check 1) — for these candidates, weight
education, projects, and certifications appropriately in Section 9
instead. Do not assume professional employment is expected for these
career stages.

==================================================
9. RESUME SCORE — FIXED POINT RUBRIC (ADDITIVE, NOT A GUESS)
==================================================

Do NOT pick a score first and rationalize it, and do NOT derive it from
how many weaknesses fired in Section 8. Compute it as the SUM of six
independently-scored categories below. Each category has explicit point
bands tied to what is actually observable in the resume. Score every
category, then add them. The sum (0-100) IS the resumeScore — no further
adjustment, rounding to a "nice number," or gut-check override.

Work through all six categories in order. For each, decide which band the
resume falls into based on the stated criteria, and pick a specific point
value within that band (not always the top or bottom) based on how
strongly the resume matches the description.

--------------------------------------------------
A. SUBSTANTIATED WORK EVIDENCE — 0 to 35 points
--------------------------------------------------
Look at projects AND experience entries together (whichever the
candidate has, in any combination — this category does not require
employment specifically).

0-8:   No projects and no experience, OR entries present in name only
       with no description of what was done.
9-17:  At least one project or experience entry with real detail (what
       was built/done, tools or context used) but no measurable
       outcomes anywhere, OR only a single entry overall.
18-26: Multiple project/experience entries with clear detail (specific
       tools, responsibilities, scope) but few or no measurable
       outcomes.
27-35: Multiple project/experience entries with clear detail AND
       specific measurable outcomes (metrics, results, scale, accuracy
       numbers, etc.) in at least some entries.

--------------------------------------------------
B. SKILL EVIDENCE RATIO — 0 to 20 points
--------------------------------------------------
Compare the total "skills" array against how many of those skills are
actually corroborated by a project, experience, certification, or
education entry (per Check 4's logic — generic soft-skill labels need
real corroboration, not just presence in the list).

0-5:   Majority of listed skills are unsupported anywhere in the resume.
6-12:  Roughly half the skills are supported, half are not.
13-17: Most skills are supported; only one or two are unsupported or
       purely aspirational.
18-20: Nearly all listed skills are clearly demonstrated somewhere in
       the resume.

--------------------------------------------------
C. EDUCATION & CERTIFICATIONS — 0 to 15 points
--------------------------------------------------
0-5:   No education and no certifications documented, OR entries present
       but unclear/incomplete.
6-10:  Education or certifications present and relevant, but limited
       (e.g., only one entry, or in-progress with nothing else to
       substantiate it).
11-15: Clear, relevant, reasonably complete education and/or
       certifications that support the candidate's apparent direction.

Do not penalize a resume for lacking certifications if education is
solid and relevant, or vice versa — score what is actually present on
its own merits, not against an assumption that both should exist.

--------------------------------------------------
D. ACHIEVEMENT / IMPACT CLARITY — 0 to 10 points
--------------------------------------------------
0-3:   No measurable outcomes anywhere in the resume (no metrics,
       numbers, results, or scale mentioned for any project, role, or
       achievement).
4-7:   Some measurable outcomes present, but limited to one or two
       entries or inconsistent.
8-10:  Clear, specific, credible measurable outcomes present across
       multiple entries.

--------------------------------------------------
E. CAREER POSITIONING & CLARITY — 0 to 10 points
--------------------------------------------------
0-3:   Resume lacks coherent direction, OR its framing overclaims
       relative to its actual content (see Check 1's framing-mismatch
       logic), OR reads as generic/vague without concrete specifics
       tying claims to evidence.
4-7:   Reasonably clear direction with some vagueness or minor
       inconsistency between claims and content.
8-10:  Clear, coherent, specific professional direction that is fully
       consistent with the content actually presented.

--------------------------------------------------
F. COMPLETENESS & STRUCTURE — 0 to 10 points
--------------------------------------------------
0-3:   Missing contact info, disorganized structure, or missing a
       section that is clearly relevant and expected for this
       candidate's stage/field (e.g., no projects, no certifications,
       AND no substantive experience — all three absent at once is a
       significant completeness gap regardless of career stage).
4-7:   Reasonably complete with one minor gap.
8-10:  Complete, well-organized, nothing relevant missing.

--------------------------------------------------
FINAL STEP — SUM
--------------------------------------------------
resumeScore = A + B + C + D + E + F (each already 0-100 scale by
construction since the max points sum to 100: 35+20+15+10+10+10=100).

Sanity check before finalizing: state to yourself the six sub-scores you
assigned and confirm they sum correctly to your final resumeScore. If
two different resumes have clearly different evidence quality, their
sub-scores — and therefore final scores — should differ accordingly;
if you find yourself wanting to output the same round number (70, 75,
80) you output last time regardless of this resume's specific content,
stop and re-score each category from the actual text in front of you.

Career-stage fairness is built into the rubric itself, not applied as a
separate adjustment: Category A rewards projects equally to employment,
so students/freshers with strong project work score well there without
needing paid experience. Category C rewards whatever education/
certifications are genuinely present without assuming both must exist.
Category E penalizes overclaiming (weak content dressed up as senior
experience) rather than penalizing honesty about being early-career.

==================================================
10. FINAL VALIDATION — RUN THIS BEFORE OUTPUTTING
==================================================

For EVERY item in "education": does its own text state a degree/diploma/formal program? If not, move it to certifications now.

For EVERY item in "certifications": confirm it is not duplicated in "education."

For EVERY item in "experience": can you point to an employer/dates/explicit employment wording? If not, remove it.

For EVERY item in "skills": confirm it is not an item labeled "Currently Learning" or equivalent (Section 2B), and confirm no explicitly-listed skill from the resume was silently dropped.

For EVERY item in "strengths": confirm it is corroborated by something other than the profile summary/headline alone.

For EVERY weakness: confirm it traces back to one of the 9 checks in Section 8 answering YES, and that experience-related wording avoids "limited experience"-style phrasing.

For "resumeScore": confirm you can state your six sub-scores (A through F from Section 9) and that they sum to your final resumeScore.

Then confirm: valid JSON, no markdown, no nulls, all arrays present, resumeScore is an integer 0-100, no hallucinated content, no category duplicated across arrays.

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