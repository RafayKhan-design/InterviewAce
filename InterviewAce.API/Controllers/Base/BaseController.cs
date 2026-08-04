using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAce.API.Controllers.Base;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);


            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException(
                    "User ID not found in token");
            }


            return Guid.Parse(userIdClaim);
        }
    }


    protected string CurrentUserEmail
    {
        get
        {
            return User.FindFirstValue(ClaimTypes.Email)
                ?? string.Empty;
        }
    }
}