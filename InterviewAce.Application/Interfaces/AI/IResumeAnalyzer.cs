namespace InterviewAce.Application.Interfaces.AI;

using InterviewAce.Domain.Entities;

public interface IResumeAnalyzer
{
    Task<ResumeAnalysis> AnalyzeAsync(string extractedText);
}