using DocumentFormat.OpenXml.Packaging;

namespace InterviewAce.Infrastructure.Services.Extraction;

public class DocxTextExtractor
{
    public Task<string> ExtractAsync(string filePath)
    {
        using var document = WordprocessingDocument.Open(
            filePath,
            false
        );


        var body = document.MainDocumentPart?
            .Document?
            .Body;


        var text = body?.InnerText ?? string.Empty;


        return Task.FromResult(text);
    }
}