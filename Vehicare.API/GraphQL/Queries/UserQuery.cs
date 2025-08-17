using System.Security.Claims;
using HotChocolate;
using Vehicare.API.Models;
using Vehicare.API.Services;

namespace Vehicare.API.GraphQL.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    private const string ApiStatus = "API is running";

    public async Task<User?> GetMe([Service] IUserService userService, ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return await userService.GetUserByIdAsync(userId);
        }
        return null;
    }

    public string GetStatus() => ApiStatus;
}
