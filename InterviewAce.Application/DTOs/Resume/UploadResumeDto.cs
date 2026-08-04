using Microsoft.AspNetCore.Http;

namespace InterviewAce.Application.DTOs.Resume;

public class UploadResumeDto
{
    public IFormFile File { get; set; } = null!;
}