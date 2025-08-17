using HotChocolate;
using Vehicare.API.Models;
using Vehicare.API.Services;
using Vehicare.API.GraphQL.Inputs;
using System.Security.Claims;

namespace Vehicare.API.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class VehicleMutation
{
    public async Task<CreateVehiclePayload> CreateVehicle(
        CreateVehicleInput input,
        [Service] IVehicleService vehicleService,
        ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return new CreateVehiclePayload { Error = "User not authenticated" };
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(input.Make) ||
                string.IsNullOrWhiteSpace(input.Model) ||
                string.IsNullOrWhiteSpace(input.VIN))
            {
                return new CreateVehiclePayload { Error = "Make, Model, and VIN are required" };
            }

            if (input.Year < 1900 || input.Year > DateTime.Now.Year + 1)
            {
                return new CreateVehiclePayload { Error = "Invalid year provided" };
            }

            // Check if VIN already exists
            if (await vehicleService.VehicleExistsByVINAsync(input.VIN))
            {
                return new CreateVehiclePayload { Error = "A vehicle with this VIN already exists" };
            }

            // Create vehicle
            var vehicle = await vehicleService.CreateVehicleAsync(input, userId);

            return new CreateVehiclePayload { Vehicle = vehicle };
        }
        catch (Exception ex)
        {
            return new CreateVehiclePayload { Error = $"Failed to create vehicle: {ex.Message}" };
        }
    }

    public async Task<UpdateVehiclePayload> UpdateVehicle(
        UpdateVehicleInput input,
        [Service] IVehicleService vehicleService,
        ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return new UpdateVehiclePayload { Error = "User not authenticated" };
            }

            // Validate year if provided
            if (input.Year.HasValue && (input.Year < 1900 || input.Year > DateTime.Now.Year + 1))
            {
                return new UpdateVehiclePayload { Error = "Invalid year provided" };
            }

            // Check if VIN already exists (if updating VIN)
            if (!string.IsNullOrEmpty(input.VIN))
            {
                // Get the current vehicle first
                var existingVehicle = await vehicleService.GetVehicleByIdAsync(input.Id);
                if (existingVehicle == null)
                {
                    return new UpdateVehiclePayload { Error = "Vehicle not found" };
                }

                // Only check for VIN conflicts if we're changing the VIN
                if (!string.Equals(existingVehicle.VIN, input.VIN, StringComparison.OrdinalIgnoreCase))
                {
                    if (await vehicleService.VehicleExistsByVINAsync(input.VIN))
                    {
                        return new UpdateVehiclePayload { Error = "A vehicle with this VIN already exists" };
                    }
                }
            }

            // Update vehicle
            var vehicle = await vehicleService.UpdateVehicleAsync(input, userId);
            if (vehicle == null)
            {
                return new UpdateVehiclePayload { Error = "Vehicle not found or you don't have permission to update it" };
            }

            return new UpdateVehiclePayload { Vehicle = vehicle };
        }
        catch (Exception ex)
        {
            return new UpdateVehiclePayload { Error = $"Failed to update vehicle: {ex.Message}" };
        }
    }

    public async Task<DeleteVehiclePayload> DeleteVehicle(
        int vehicleId,
        [Service] IVehicleService vehicleService,
        ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return new DeleteVehiclePayload { Error = "User not authenticated" };
            }

            var success = await vehicleService.DeleteVehicleAsync(vehicleId, userId);
            if (!success)
            {
                return new DeleteVehiclePayload { Error = "Vehicle not found or you don't have permission to delete it" };
            }

            return new DeleteVehiclePayload
            {
                Success = true,
                DeletedVehicleId = vehicleId
            };
        }
        catch (Exception ex)
        {
            return new DeleteVehiclePayload { Error = $"Failed to delete vehicle: {ex.Message}" };
        }
    }
}
