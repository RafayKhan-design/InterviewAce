namespace InterviewAce.Application.DTOs.Profile;

public class CreateProfileDto
{
    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public string? Bio { get; set; }

    public int YearsOfExperience { get; set; }

    public string? Education { get; set; }
}