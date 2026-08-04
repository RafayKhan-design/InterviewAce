namespace InterviewAce.Application.Interfaces.Authentication;

public interface ITokenService
{
    string GenerateToken(
        Guid userId,
        string email
    );
}