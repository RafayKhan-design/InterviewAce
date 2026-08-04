using System.Security.Cryptography;
using InterviewAce.Application.Interfaces.Authentication;

namespace InterviewAce.Infrastructure.Services.Authentication;

public class RefreshTokenService : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);


        return Convert.ToBase64String(randomNumber);
    }
}