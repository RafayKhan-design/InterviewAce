namespace InterviewAce.Application.Interfaces.AI;

public interface IAIProvider
{
    Task<string> GenerateResponseAsync(
        string prompt
    );
}