using HotChocolate;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;
using Vehicare.API.Services;

namespace Vehicare.API.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class UserMutation
{
    public async Task<LoginPayload> LoginAsync(
        LoginInput input,
        [Service] IUserService userService,
        [Service] IJwtService jwtService)
    {
        try
        {
            // Find user by email
            var user = await userService.GetUserByEmailAsync(input.Email);
            if (user == null)
            {
                return new LoginPayload { Error = "Invalid email or password" };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return new LoginPayload { Error = "Account is disabled" };
            }

            // Validate password
            var isValidPassword = await userService.ValidatePasswordAsync(input.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                return new LoginPayload { Error = "Invalid email or password" };
            }

            // Update last login
            await userService.UpdateLastLoginAsync(user.Id);

            // Generate JWT token
            var token = jwtService.GenerateToken(user);

            return new LoginPayload
            {
                Token = token,
                User = user
            };
        }
        catch (Exception ex)
        {
            return new LoginPayload { Error = $"Login failed: {ex.Message}" };
        }
    }

    public async Task<RegisterPayload> RegisterAsync(
        RegisterInput input,
        [Service] IUserService userService)
    {
        try
        {
            // Check if user already exists
            var existingUser = await userService.GetUserByEmailAsync(input.Email);
            if (existingUser != null)
            {
                return new RegisterPayload { Error = "User with this email already exists" };
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(input.Email) ||
                string.IsNullOrWhiteSpace(input.Password) ||
                string.IsNullOrWhiteSpace(input.FirstName) ||
                string.IsNullOrWhiteSpace(input.LastName))
            {
                return new RegisterPayload { Error = "All fields are required" };
            }

            if (input.Password.Length < 6)
            {
                return new RegisterPayload { Error = "Password must be at least 6 characters long" };
            }

            // Create user
            var user = await userService.CreateUserAsync(input);

            return new RegisterPayload { User = user };
        }
        catch (Exception ex)
        {
            return new RegisterPayload { Error = $"Registration failed: {ex.Message}" };
        }
    }
}
