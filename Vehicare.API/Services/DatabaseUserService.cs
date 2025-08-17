using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Vehicare.API.Data;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;

namespace Vehicare.API.Services;

public class DatabaseUserService : IUserService
{
    private readonly AppDbContext _context;

    public DatabaseUserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateUserAsync(RegisterInput input)
    {
        var user = new User
        {
            Email = input.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password),
            FirstName = input.FirstName,
            LastName = input.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public Task<bool> ValidatePasswordAsync(string password, string hash)
    {
        var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        return Task.FromResult(isValid);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
