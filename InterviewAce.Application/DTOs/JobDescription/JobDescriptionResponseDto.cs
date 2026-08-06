namespace InterviewAce.Application.DTOs.JobDescription;

public class JobDescriptionResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}