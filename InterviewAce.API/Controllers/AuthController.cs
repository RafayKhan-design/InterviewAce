using InterviewAce.Application.DTOs.Authentication;
using InterviewAce.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InterviewAce.Application.DTOs.Common;

namespace InterviewAce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;


    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">User registration details including email and password.</param>
    /// <returns>A success response when the account is created.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result)
        {
            return BadRequest(
                new ApiResponseDto<object>(
                    false,
                    "Email already exists."
                )
            );
        }


        return Ok(
            new ApiResponseDto<object>(
                true,
                "User registered successfully."
            )
        );
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="request">User email and password.</param>
    /// <returns>Access token and refresh token information.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(
                new ApiResponseDto<object>(
                    false,
                    "Invalid email or password."
                )
            );
        }


        return Ok(
            new ApiResponseDto<LoginResponseDto>(
                true,
                "Login successful.",
                result
            )
        );
    }

    /// <summary>
    /// Returns the currently authenticated user's information.
    /// </summary>
    /// <returns>User ID and email extracted from JWT claims.</returns>
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var email = User.FindFirstValue(ClaimTypes.Email);


        return Ok(
            new ApiResponseDto<object>(
                true,
                "Current user retrieved successfully.",
                new
                {
                    UserId = userId,
                    Email = email
                }
            )
        );
    }

    /// <summary>
    /// Generates a new JWT token using a valid refresh token.
    /// </summary>
    /// <param name="request">Refresh token request.</param>
    /// <returns>New access and refresh tokens.</returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
    RefreshTokenRequestDto request)
    {
        var result = await _authService
            .RefreshTokenAsync(request.RefreshToken);


        if (result == null)
        {
            return Unauthorized(
                new ApiResponseDto<object>(
                    false,
                    "Invalid or expired refresh token."
                )
            );
        }


        return Ok(
            new ApiResponseDto<LoginResponseDto>(
                true,
                "Token refreshed successfully.",
                result
            )
        );
    }
}