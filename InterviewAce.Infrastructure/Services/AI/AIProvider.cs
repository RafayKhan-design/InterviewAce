using InterviewAce.Application.Interfaces.AI;

namespace InterviewAce.Infrastructure.Services.AI;

public class AIProvider : IAIProvider
{
    public Task<string> GenerateResponseAsync(
        string prompt)
    {
        // Temporary AI provider implementation.
        // Later this will connect with Gemini/OpenAI/Ollama.

        return Task.FromResult(
            string.Empty
        );
    }
}