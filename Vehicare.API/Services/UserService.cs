using BCrypt.Net;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;

namespace Vehicare.API.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(RegisterInput input);
    Task<bool> ValidatePasswordAsync(string password, string hash);
    Task UpdateLastLoginAsync(int userId);
}

public class UserService : IUserService
{
    // In a real application, you would use a database context here
    // For this example, we'll use an in-memory list
    private readonly List<User> _users = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public Task<User?> GetUserByEmailAsync(string email)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }
    }

    public Task<User?> GetUserByIdAsync(int id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
    }

    public Task<User> CreateUserAsync(RegisterInput input)
    {
        lock (_lock)
        {
            var user = new User
            {
                Id = _nextId++,
                Email = input.Email.ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password),
                FirstName = input.FirstName,
                LastName = input.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _users.Add(user);
            return Task.FromResult(user);
        }
    }

    public Task<bool> ValidatePasswordAsync(string password, string hash)
    {
        var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        return Task.FromResult(isValid);
    }

    public Task UpdateLastLoginAsync(int userId)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
    }
}
