using System.Text;
using System.Text.Json;
using InterviewAce.Application.Interfaces.AI;
using Microsoft.Extensions.Configuration;

namespace InterviewAce.Infrastructure.Services.AI;

public class GroqAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;


    public GroqAIProvider(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }



    public async Task<string> GenerateResponseAsync(string prompt)
    {
        var apiKey = _configuration["AI:GroqApiKey"];


        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",

            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };


        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );


        request.Headers.Add(
            "Authorization",
            $"Bearer {apiKey}"
        );


        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );


        var response = await _httpClient.SendAsync(request);


        var result = await response.Content.ReadAsStringAsync();


        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Groq API Error: {result}"
            );
        }


        using var document =
            JsonDocument.Parse(result);


        return document
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;
    }
}