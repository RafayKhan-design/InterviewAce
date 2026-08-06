namespace InterviewAce.Infrastructure.Services.Extraction;

public class TxtTextExtractor
{
    public async Task<string> ExtractAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
}