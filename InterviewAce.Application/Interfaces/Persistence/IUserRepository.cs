using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterviewAce.Domain.Entities;

namespace InterviewAce.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);

    Task AddRefreshTokenAsync(RefreshToken refreshToken);

    Task<User?> GetByRefreshTokenAsync(string refreshToken);

    Task SaveChangesAsync();
}