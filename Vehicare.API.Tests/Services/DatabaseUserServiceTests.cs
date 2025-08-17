using FluentAssertions;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;
using Vehicare.API.Services;
using Vehicare.API.Tests.Helpers;
using Xunit;

namespace Vehicare.API.Tests.Services;

public class DatabaseUserServiceTests : IDisposable
{
    private readonly DatabaseUserService _userService;
    private readonly Data.AppDbContext _context;

    public DatabaseUserServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _userService = new DatabaseUserService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _userService.GetUserByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserByEmailAsync("TEST@EXAMPLE.COM");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUserWithHashedPassword()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _userService.CreateUserAsync(registerInput);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.PasswordHash.Should().NotBe("password123");
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify password is hashed correctly
        BCrypt.Net.BCrypt.Verify("password123", result.PasswordHash).Should().BeTrue();

        // Verify user is saved to database
        var savedUser = await _context.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldNormalizeEmail()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Email = "TEST@EXAMPLE.COM",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _userService.CreateUserAsync(registerInput);

        // Assert
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "password123";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var result = await _userService.ValidatePasswordAsync(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "password123";
        var incorrectPassword = "wrongpassword";
        var hash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        // Act
        var result = await _userService.ValidatePasswordAsync(incorrectPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateLastLoginAsync_WhenUserExists_ShouldUpdateLastLoginTime()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _userService.UpdateLastLoginAsync(user.Id);

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.LastLoginAt.Should().NotBeNull();
        updatedUser.LastLoginAt.Should().BeCloseTo(beforeUpdate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateLastLoginAsync_WhenUserDoesNotExist_ShouldNotThrowException()
    {
        // Act & Assert
        var act = async () => await _userService.UpdateLastLoginAsync(999);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _userService.CreateUserAsync(registerInput);

        // Assert - Verify data persisted by querying directly from context
        var userFromDb = await _context.Users.FindAsync(result.Id);
        userFromDb.Should().NotBeNull();
        userFromDb!.Email.Should().Be("test@example.com");
        userFromDb.FirstName.Should().Be("John");
        userFromDb.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldQueryDatabase()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create a new service instance to ensure we're querying the database
        using var newContext = TestDbContextFactory.CreateInMemoryContext();
        var testUser = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        newContext.Users.Add(testUser);
        await newContext.SaveChangesAsync();

        var newService = new DatabaseUserService(newContext);

        // Act
        var result = await newService.GetUserByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task UpdateLastLoginAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _userService.UpdateLastLoginAsync(user.Id);

        // Assert - Verify changes persisted by querying directly from context
        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.LastLoginAt.Should().NotBeNull();
    }
}
