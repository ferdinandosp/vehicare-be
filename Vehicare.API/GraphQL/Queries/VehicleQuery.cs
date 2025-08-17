using HotChocolate;
using Vehicare.API.Models;
using Vehicare.API.Services;
using System.Security.Claims;

namespace Vehicare.API.GraphQL.Queries;

[ExtendObjectType("Query")]
public class VehicleQuery
{
    public async Task<Vehicle?> GetVehicle(
        int id,
        [Service] IVehicleService vehicleService,
        ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        var vehicle = await vehicleService.GetVehicleByIdAsync(id);

        // Ensure the vehicle belongs to the authenticated user
        if (vehicle?.UserId != userId)
        {
            return null;
        }

        return vehicle;
    }

    public async Task<IEnumerable<Vehicle>> GetMyVehicles(
        [Service] IVehicleService vehicleService,
        ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Enumerable.Empty<Vehicle>();
        }

        return await vehicleService.GetUserVehiclesAsync(userId);
    }
}
