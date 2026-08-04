using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Domain.Entities;
using InterviewAce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;


    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == email);
    }


    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }


    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(
                x => x.RefreshTokens.Any(
                    r => r.Token == refreshToken
                )
            );
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}