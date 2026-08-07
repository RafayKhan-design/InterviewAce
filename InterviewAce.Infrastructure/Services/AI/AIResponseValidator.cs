using InterviewAce.Infrastructure.Services.AI.Models;

public static class AIResponseValidator
{
    public static ResumeAIResponse Validate(ResumeAIResponse response)
    {
        response.Skills = Clean(response.Skills);
        response.Projects = Clean(response.Projects);
        response.Certifications = Clean(response.Certifications);
        response.Strengths = Clean(response.Strengths);
        response.Weaknesses = Clean(response.Weaknesses);
        response.Experience = Clean(response.Experience);
        response.Education = Clean(response.Education);

        response.ResumeScore = Math.Clamp(response.ResumeScore, 0, 100);

        return response;
    }

    private static List<string> Clean(List<string>? items)
    {
        if (items == null)
            return new();

        return items
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();
    }
}