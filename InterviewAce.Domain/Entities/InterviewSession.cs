namespace InterviewAce.Domain.Entities;

public class InterviewSession
{
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }

    public Interview Interview { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = "InProgress";

    public ICollection<InterviewAnswer> Answers { get; set; }
        = new List<InterviewAnswer>();
}