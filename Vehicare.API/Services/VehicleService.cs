using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;

namespace Vehicare.API.Services;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(int userId);
    Task<Vehicle> CreateVehicleAsync(CreateVehicleInput input, int userId);
    Task<Vehicle?> UpdateVehicleAsync(UpdateVehicleInput input, int userId);
    Task<bool> DeleteVehicleAsync(int vehicleId, int userId);
    Task<bool> VehicleExistsByVINAsync(string vin);
}

public class VehicleService : IVehicleService
{
    // In a real application, you would use a database context here
    // For this example, we'll use an in-memory list
    private readonly List<Vehicle> _vehicles = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        lock (_lock)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
            return Task.FromResult(vehicle);
        }
    }

    public Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(int userId)
    {
        lock (_lock)
        {
            var userVehicles = _vehicles.Where(v => v.UserId == userId).ToList();
            return Task.FromResult<IEnumerable<Vehicle>>(userVehicles);
        }
    }

    public Task<Vehicle> CreateVehicleAsync(CreateVehicleInput input, int userId)
    {
        lock (_lock)
        {
            var vehicle = new Vehicle
            {
                Id = _nextId++,
                UserId = userId,
                Make = input.Make,
                Model = input.Model,
                Year = input.Year,
                VIN = input.VIN,
                LicensePlate = input.LicensePlate,
                CurrentMileage = input.CurrentMileage,
                Color = input.Color,
                PurchaseDate = input.PurchaseDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _vehicles.Add(vehicle);
            return Task.FromResult(vehicle);
        }
    }

    public Task<Vehicle?> UpdateVehicleAsync(UpdateVehicleInput input, int userId)
    {
        lock (_lock)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == input.Id && v.UserId == userId);
            if (vehicle == null)
            {
                return Task.FromResult<Vehicle?>(null);
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(input.Make))
                vehicle.Make = input.Make;
            if (!string.IsNullOrEmpty(input.Model))
                vehicle.Model = input.Model;
            if (input.Year.HasValue)
                vehicle.Year = input.Year.Value;
            if (!string.IsNullOrEmpty(input.VIN))
                vehicle.VIN = input.VIN;
            if (!string.IsNullOrEmpty(input.LicensePlate))
                vehicle.LicensePlate = input.LicensePlate;
            if (input.CurrentMileage.HasValue)
                vehicle.CurrentMileage = input.CurrentMileage.Value;
            if (!string.IsNullOrEmpty(input.Color))
                vehicle.Color = input.Color;
            if (input.PurchaseDate.HasValue)
                vehicle.PurchaseDate = input.PurchaseDate.Value;

            vehicle.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<Vehicle?>(vehicle);
        }
    }

    public Task<bool> DeleteVehicleAsync(int vehicleId, int userId)
    {
        lock (_lock)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId && v.UserId == userId);
            if (vehicle == null)
            {
                return Task.FromResult(false);
            }

            _vehicles.Remove(vehicle);
            return Task.FromResult(true);
        }
    }

    public Task<bool> VehicleExistsByVINAsync(string vin)
    {
        lock (_lock)
        {
            var exists = _vehicles.Any(v => v.VIN.Equals(vin, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}
