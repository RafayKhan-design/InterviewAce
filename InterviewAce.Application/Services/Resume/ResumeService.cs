using InterviewAce.Application.DTOs.Resume;
using InterviewAce.Application.Interfaces;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Application.Interfaces.Resume;
using InterviewAce.Application.Interfaces.Storage;
using ResumeEntity = InterviewAce.Domain.Entities.Resume;

namespace InterviewAce.Application.Services.Resume;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IFileStorageService _fileStorageService;


    public ResumeService(
        IResumeRepository resumeRepository,
        IFileStorageService fileStorageService)
    {
        _resumeRepository = resumeRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ResumeResponseDto> UploadAsync(
    Guid userId,
    UploadResumeDto request)
    {
        var file = request.File;


        if (file.Length == 0)
        {
            throw new Exception("File cannot be empty.");
        }


        var allowedExtensions = new[]
        {
        ".pdf",
        ".doc",
        ".docx"
    };


        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();


        if (!allowedExtensions.Contains(extension))
        {
            throw new Exception(
                "Only PDF, DOC and DOCX files are allowed."
            );
        }


        const long maxFileSize = 5 * 1024 * 1024; // 5 MB


        if (file.Length > maxFileSize)
        {
            throw new Exception(
                "File size cannot exceed 5 MB."
            );
        }


        // Resume limit check
        var resumeCount = await _resumeRepository
            .GetResumeCountAsync(userId);


        if (resumeCount >= 5)
        {
            throw new InvalidOperationException(
                "You have reached the maximum limit of 5 resumes. Please delete an existing resume before uploading another."
            );
        }



        var filePath = await _fileStorageService.UploadAsync(
            file,
            "resumes"
        );



        var resume = new ResumeEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FileName = file.FileName,
            FilePath = filePath,
            FileType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };


        await _resumeRepository.AddAsync(resume);

        await _resumeRepository.SaveChangesAsync();



        return new ResumeResponseDto
        {
            Id = resume.Id,
            FileName = resume.FileName,
            FileType = resume.FileType,
            FileSize = resume.FileSize,
            UploadedAt = resume.UploadedAt
        };
    }



    public async Task<List<ResumeResponseDto>> GetMyResumesAsync(
    Guid userId)
    {
        var resumes = await _resumeRepository
            .GetByUserIdAsync(userId);


        return resumes
            .Select(x => new ResumeResponseDto
            {
                Id = x.Id,
                FileName = x.FileName,
                FileType = x.FileType,
                FileSize = x.FileSize,
                UploadedAt = x.UploadedAt
            })
            .ToList();
    }



    public async Task<bool> DeleteAsync(
    Guid userId,
    Guid resumeId)
    {
        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);


        if (resume == null || resume.UserId != userId)
        {
            return false;
        }


        await _fileStorageService
            .DeleteAsync(resume.FilePath);


        _resumeRepository.Delete(resume);


        await _resumeRepository.SaveChangesAsync();


        return true;
    }
}