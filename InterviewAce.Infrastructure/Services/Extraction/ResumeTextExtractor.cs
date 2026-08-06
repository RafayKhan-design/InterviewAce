using InterviewAce.Application.Interfaces.Extraction;
using InterviewAce.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Hosting;

namespace InterviewAce.Infrastructure.Services.Extraction;

public class ResumeTextExtractor : IResumeTextExtractor
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IWebHostEnvironment _environment;

    private readonly PdfTextExtractor _pdfExtractor;
    private readonly DocxTextExtractor _docxExtractor;
    private readonly TxtTextExtractor _txtExtractor;


    public ResumeTextExtractor(
    PdfTextExtractor pdfExtractor,
    DocxTextExtractor docxExtractor,
    TxtTextExtractor txtExtractor,
    IFileStorageService fileStorageService,
    IWebHostEnvironment environment)
    {
        _pdfExtractor = pdfExtractor;
        _docxExtractor = docxExtractor;
        _txtExtractor = txtExtractor;

        _fileStorageService = fileStorageService;
        _environment = environment;
    }



    public async Task<string> ExtractTextAsync(
    string filePath,
    string fileType)
    {
        var fullPath = Path.Combine(
            _environment.WebRootPath,
            filePath
        );


        fileType = fileType.ToLower();


        if (fileType.Contains("pdf"))
        {
            return await _pdfExtractor.ExtractAsync(fullPath);
        }


        if (fileType.Contains("docx") ||
           fileType.Contains("word"))
        {
            return await _docxExtractor.ExtractAsync(fullPath);
        }


        if (fileType.Contains("txt") ||
           fileType.Contains("text"))
        {
            return await _txtExtractor.ExtractAsync(fullPath);
        }


        throw new NotSupportedException(
            $"Resume format '{fileType}' is not supported."
        );
    }
}