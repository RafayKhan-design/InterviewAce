using BCrypt.Net;
using InterviewAce.Application.DTOs.Authentication;
using InterviewAce.Application.Interfaces.Authentication;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;


    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }


    public async Task<bool> RegisterAsync(RegisterDto request)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return false;
        }


        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

            CreatedAt = DateTime.UtcNow
        };


        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();


        return true;
    }



    public async Task<LoginResponseDto?> LoginAsync(LoginDto request)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Email);


        if (user == null)
        {
            return null;
        }


        bool passwordValid = BCrypt.Net.BCrypt
            .Verify(request.Password, user.PasswordHash);


        if (!passwordValid)
        {
            return null;
        }


        var accessToken = _tokenService.GenerateToken(
            user.Id,
            user.Email
        );


        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            Token = _refreshTokenService.GenerateRefreshToken(),

            ExpiresAt = DateTime.UtcNow.AddDays(7),

            CreatedAt = DateTime.UtcNow
        };


        await _userRepository.AddRefreshTokenAsync(refreshToken);

        await _userRepository.SaveChangesAsync();


        return new LoginResponseDto
        {
            AccessToken = accessToken,

            RefreshToken = refreshToken.Token,

            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(15),

            RefreshTokenExpiration = refreshToken.ExpiresAt
        };
    }




    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository
            .GetByRefreshTokenAsync(refreshToken);


        if (user == null)
        {
            return null;
        }


        var existingToken = user.RefreshTokens
            .FirstOrDefault(x => x.Token == refreshToken);


        if (existingToken == null)
        {
            return null;
        }


        if (existingToken.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }


        if (existingToken.RevokedAt != null)
        {
            return null;
        }


        // Revoke old refresh token
        existingToken.RevokedAt = DateTime.UtcNow;



        var newAccessToken = _tokenService.GenerateToken(
            user.Id,
            user.Email
        );


        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            Token = _refreshTokenService.GenerateRefreshToken(),

            ExpiresAt = DateTime.UtcNow.AddDays(7),

            CreatedAt = DateTime.UtcNow
        };


        await _userRepository.AddRefreshTokenAsync(newRefreshToken);

        await _userRepository.SaveChangesAsync();



        return new LoginResponseDto
        {
            AccessToken = newAccessToken,

            RefreshToken = newRefreshToken.Token,

            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(15),

            RefreshTokenExpiration = newRefreshToken.ExpiresAt
        };
    }
}