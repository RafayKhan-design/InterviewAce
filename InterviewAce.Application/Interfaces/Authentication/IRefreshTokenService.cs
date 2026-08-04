using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAce.Application.Interfaces.Authentication
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken();
    }
}
