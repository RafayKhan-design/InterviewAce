using InterviewAce.Application.DTOs.Resume;

namespace InterviewAce.Application.Interfaces.Resume;

public interface IResumeService
{
    Task<ResumeResponseDto> UploadAsync(
        Guid userId,
        UploadResumeDto request);


    Task<List<ResumeResponseDto>> GetMyResumesAsync(
        Guid userId);


    Task<bool> DeleteAsync(
        Guid userId,
        Guid resumeId);

    Task<(byte[] FileBytes, string FileType, string FileName)?> DownloadAsync(
    Guid userId,
    Guid resumeId);
}