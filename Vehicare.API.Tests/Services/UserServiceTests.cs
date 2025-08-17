using FluentAssertions;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;
using Vehicare.API.Services;
using Xunit;

namespace Vehicare.API.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };
        await _userService.CreateUserAsync(registerInput);

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
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };
        await _userService.CreateUserAsync(registerInput);

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
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };
        var createdUser = await _userService.CreateUserAsync(registerInput);

        // Act
        var result = await _userService.GetUserByIdAsync(createdUser.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdUser.Id);
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
        var registerInput = new RegisterInput
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };
        var user = await _userService.CreateUserAsync(registerInput);
        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _userService.UpdateLastLoginAsync(user.Id);

        // Assert
        var updatedUser = await _userService.GetUserByIdAsync(user.Id);
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
    public async Task CreateUserAsync_ShouldAssignUniqueIds()
    {
        // Arrange
        var input1 = new RegisterInput { Email = "user1@example.com", Password = "pass1", FirstName = "User", LastName = "One" };
        var input2 = new RegisterInput { Email = "user2@example.com", Password = "pass2", FirstName = "User", LastName = "Two" };

        // Act
        var user1 = await _userService.CreateUserAsync(input1);
        var user2 = await _userService.CreateUserAsync(input2);

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        user1.Id.Should().BeGreaterThan(0);
        user2.Id.Should().BeGreaterThan(0);
    }
}
