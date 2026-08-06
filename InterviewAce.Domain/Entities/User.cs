namespace InterviewAce.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }


    // Refresh Tokens
    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();


    // Candidate Profile
    public CandidateProfile? CandidateProfile { get; set; }

    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();

    public ICollection<JobDescription> JobDescriptions { get; set; }
    = new List<JobDescription>();
}