using InterviewAce.Application.Configurations;
using InterviewAce.Application.DTOs.AI;
using InterviewAce.Application.Interfaces.AI;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace InterviewAce.Infrastructure.Services.AI;

public class GeminiAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly AISettings _settings;


    public GeminiAIProvider(
        HttpClient httpClient,
        IOptions<AISettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }



    public async Task<string> GenerateResponseAsync(
        string prompt)
    {
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";


        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };


        var json = JsonSerializer.Serialize(
            requestBody
        );


        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );


        var response = await _httpClient.PostAsync(
            url,
            content
        );


        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"Gemini API Error: {error}"
            );
        }


        var responseJson = await response.Content
    .ReadAsStringAsync();


        var geminiResponse =
            JsonSerializer.Deserialize<GeminiResponseDto>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


        return geminiResponse?
            .Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text
            ?? string.Empty;



    }
}