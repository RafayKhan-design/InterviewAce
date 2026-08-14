using InterviewAce.Application.DTOs.Interview;
using InterviewAce.Application.Interfaces.Interview;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Services.Interview;

public class InterviewAnswerService : IInterviewAnswerService
{
    private readonly IInterviewAnswerRepository _repository;
    private readonly IInterviewSessionRepository _sessionRepository;

    public InterviewAnswerService(
        IInterviewAnswerRepository repository,
        IInterviewSessionRepository sessionRepository)
    {
        _repository = repository;
        _sessionRepository = sessionRepository;
    }

    public async Task<InterviewAnswerResponseDto> SubmitAsync(
        Guid userId,
        Guid sessionId,
        SubmitInterviewAnswerDto request)
    {
        var session = await _sessionRepository.GetByIdAndUserIdAsync(
    sessionId,
    userId);

        if (session == null)
        {
            throw new KeyNotFoundException(
                "Interview session not found.");
        }

        var questionExists = session.Interview.Questions
            .Any(q => q.Id == request.InterviewQuestionId);

        if (!questionExists)
        {
            throw new KeyNotFoundException(
                "Interview question does not belong to this interview.");
        }

        if (string.IsNullOrWhiteSpace(request.AnswerText))
        {
            throw new ArgumentException(
                "Answer cannot be empty.");
        }

        var answer = new InterviewAnswer
        {
            Id = Guid.NewGuid(),
            InterviewSessionId = sessionId,
            InterviewQuestionId = request.InterviewQuestionId,
            AnswerText = request.AnswerText.Trim(),
            AnsweredAt = DateTime.UtcNow
        };

        await _repository.AddAsync(answer);
        await _repository.SaveChangesAsync();

        return MapToResponse(answer);
    }

    public async Task<List<InterviewAnswerResponseDto>> GetBySessionIdAsync(
        Guid userId,
        Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAndUserIdAsync(
    sessionId,
    userId);

        if (session == null)
        {
            throw new KeyNotFoundException(
                "Interview session not found.");
        }

        var answers = await _repository.GetBySessionIdAsync(
            sessionId,
            userId);

        return answers
            .Select(MapToResponse)
            .ToList();
    }

    private static InterviewAnswerResponseDto MapToResponse(
        InterviewAnswer answer)
    {
        return new InterviewAnswerResponseDto
        {
            Id = answer.Id,
            InterviewSessionId = answer.InterviewSessionId,
            InterviewQuestionId = answer.InterviewQuestionId,
            Answer = answer.AnswerText,
            SubmittedAt = answer.AnsweredAt
        };
    }
}