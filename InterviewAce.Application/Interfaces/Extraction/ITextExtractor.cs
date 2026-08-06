namespace InterviewAce.Application.Interfaces.Extraction;

public interface ITextExtractor
{
    bool CanHandle(string fileType);


    Task<string> ExtractTextAsync(
        string filePath);
}