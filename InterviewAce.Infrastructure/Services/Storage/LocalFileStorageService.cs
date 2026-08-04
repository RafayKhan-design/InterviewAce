using InterviewAce.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InterviewAce.Infrastructure.Services.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;


    public LocalFileStorageService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }



    public async Task<string> UploadAsync(
    IFormFile file,
    string folderName)
    {
        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            folderName
        );


        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }


        var extension = Path.GetExtension(file.FileName);


        var fileName = $"{Guid.NewGuid()}{extension}";


        var filePath = Path.Combine(
            uploadsFolder,
            fileName
        );


        using var stream = new FileStream(
            filePath,
            FileMode.Create
        );


        await file.CopyToAsync(stream);


        return Path.Combine(
            folderName,
            fileName
        ).Replace("\\", "/");
    }



    public Task DeleteAsync(
        string filePath)
    {
        var fullPath = Path.Combine(
            _environment.WebRootPath,
            filePath
        );


        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }


        return Task.CompletedTask;
    }
}