namespace InterviewAce.Application.Interfaces.Extraction;

public interface IResumeTextExtractor
{
    Task<string> ExtractTextAsync(
        string filePath,
        string fileType);
}