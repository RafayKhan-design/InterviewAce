namespace InterviewAce.Domain.Entities;

public class CandidateProfile
{
    public Guid Id { get; set; }


    public Guid UserId { get; set; }


    public string FullName { get; set; } = string.Empty;


    public string? Phone { get; set; }


    public string? LinkedInUrl { get; set; }


    public string? GitHubUrl { get; set; }


    public string? Bio { get; set; }


    public int YearsOfExperience { get; set; }


    public string? Education { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime? UpdatedAt { get; set; }


    // Navigation

    public User User { get; set; } = null!;
}