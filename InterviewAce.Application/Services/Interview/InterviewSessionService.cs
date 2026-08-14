using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using InterviewAce.Application.Interfaces.Persistence;

namespace InterviewAce.Application.Services.Interview;

public class InterviewSessionService : IInterviewSessionService
{
    private readonly IInterviewSessionRepository _sessionRepository;

    public InterviewSessionService(
        IInterviewSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<InterviewSessionResponseDto> StartAsync(
        Guid userId,
        Guid interviewId)
    {
        var session = new Domain.Entities.InterviewSession
        {
            Id = Guid.NewGuid(),
            InterviewId = interviewId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            Status = "InProgress"
        };

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return MapToResponse(session);
    }

    public async Task<InterviewSessionResponseDto?> GetByIdAsync(
        Guid userId,
        Guid sessionId)
    {
        var session = await _sessionRepository
            .GetByIdAndUserIdAsync(sessionId, userId);

        if (session == null)
        {
            return null;
        }

        return MapToResponse(session);
    }

    public async Task<List<InterviewSessionResponseDto>> GetAllAsync(
        Guid userId)
    {
        var sessions = await _sessionRepository
            .GetByUserIdAsync(userId);

        return sessions
            .Select(MapToResponse)
            .ToList();
    }

    private static InterviewSessionResponseDto MapToResponse(
        Domain.Entities.InterviewSession session)
    {
        return new InterviewSessionResponseDto
        {
            Id = session.Id,
            InterviewId = session.InterviewId,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            Status = session.Status
        };
    }
}