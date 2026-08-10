namespace InterviewAce.Domain.Entities;

public class JobDescription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}