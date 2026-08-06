using UglyToad.PdfPig;

namespace InterviewAce.Infrastructure.Services.Extraction;

public class PdfTextExtractor
{
    public Task<string> ExtractAsync(string filePath)
    {
        using var document = PdfDocument.Open(filePath);

        var text = string.Empty;


        foreach (var page in document.GetPages())
        {
            text += page.Text;
        }


        return Task.FromResult(text);
    }
}