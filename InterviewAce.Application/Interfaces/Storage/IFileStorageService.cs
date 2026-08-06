using Microsoft.AspNetCore.Http;

namespace InterviewAce.Application.Interfaces.Storage;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        IFormFile file,
        string folderName);


    Task DeleteAsync(
        string filePath);

    Task<byte[]> DownloadAsync(
    string filePath);
}