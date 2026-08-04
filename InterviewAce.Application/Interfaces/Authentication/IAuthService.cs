using InterviewAce.Application.DTOs.Authentication;

namespace InterviewAce.Application.Interfaces.Authentication;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto request);

    Task<LoginResponseDto?> LoginAsync(LoginDto request);

    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
}
