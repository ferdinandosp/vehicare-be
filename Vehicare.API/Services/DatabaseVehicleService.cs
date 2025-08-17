using Microsoft.EntityFrameworkCore;
using Vehicare.API.Data;
using Vehicare.API.Models;
using Vehicare.API.GraphQL.Inputs;

namespace Vehicare.API.Services;

public class DatabaseVehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public DatabaseVehicleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        return await _context.Vehicles
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(int userId)
    {
        return await _context.Vehicles
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<Vehicle> CreateVehicleAsync(CreateVehicleInput input, int userId)
    {
        var vehicle = new Vehicle
        {
            UserId = userId,
            Make = input.Make,
            Model = input.Model,
            Year = input.Year,
            VIN = input.VIN.ToUpperInvariant(),
            LicensePlate = input.LicensePlate,
            CurrentMileage = input.CurrentMileage,
            Color = input.Color,
            PurchaseDate = input.PurchaseDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Load the user navigation property
        await _context.Entry(vehicle)
            .Reference(v => v.User)
            .LoadAsync();

        return vehicle;
    }

    public async Task<Vehicle?> UpdateVehicleAsync(UpdateVehicleInput input, int userId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == input.Id && v.UserId == userId);

        if (vehicle == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(input.Make))
            vehicle.Make = input.Make;
        if (!string.IsNullOrEmpty(input.Model))
            vehicle.Model = input.Model;
        if (input.Year.HasValue)
            vehicle.Year = input.Year.Value;
        if (!string.IsNullOrEmpty(input.VIN))
            vehicle.VIN = input.VIN.ToUpperInvariant();
        if (!string.IsNullOrEmpty(input.LicensePlate))
            vehicle.LicensePlate = input.LicensePlate;
        if (input.CurrentMileage.HasValue)
            vehicle.CurrentMileage = input.CurrentMileage.Value;
        if (!string.IsNullOrEmpty(input.Color))
            vehicle.Color = input.Color;
        if (input.PurchaseDate.HasValue)
            vehicle.PurchaseDate = input.PurchaseDate.Value;

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Load the user navigation property
        await _context.Entry(vehicle)
            .Reference(v => v.User)
            .LoadAsync();

        return vehicle;
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId, int userId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
        {
            return false;
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> VehicleExistsByVINAsync(string vin)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.VIN.ToUpper() == vin.ToUpper());
    }
}
