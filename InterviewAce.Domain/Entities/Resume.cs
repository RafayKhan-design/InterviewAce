using System.ComponentModel.DataAnnotations;

namespace InterviewAce.Domain.Entities;

public class Resume
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }


    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;


    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;


    [MaxLength(100)]
    public string FileType { get; set; } = string.Empty;


    public long FileSize { get; set; }


    public DateTime UploadedAt { get; set; }


    // Navigation Property
    public User User { get; set; } = null!;

    // Navigation Property
    public ResumeAnalysis? ResumeAnalysis { get; set; }
}