using InterviewAce.Application.Interfaces.Processing;
using System.Text.RegularExpressions;

namespace InterviewAce.Infrastructure.Services.Processing;

public class TextCleaner : ITextCleaner
{
    public string Clean(string extractedText)
    {
        if (string.IsNullOrWhiteSpace(extractedText))
        {
            return string.Empty;
        }


        // Remove extra spaces
        var cleanedText = Regex.Replace(
            extractedText,
            @"\s+",
            " "
        );


        // Fix spaces between letters
        cleanedText = Regex.Replace(
            cleanedText,
            @"(?<=\b[A-Z])\s+(?=[A-Z]\b)",
            ""
        );


        return cleanedText.Trim();
    }
}