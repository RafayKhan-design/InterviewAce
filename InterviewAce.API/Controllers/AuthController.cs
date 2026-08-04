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