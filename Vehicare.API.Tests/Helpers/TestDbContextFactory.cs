using Microsoft.EntityFrameworkCore;
using Vehicare.API.Data;

namespace Vehicare.API.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static AppDbContext CreateInMemoryContextWithData(string? databaseName = null)
    {
        var context = CreateInMemoryContext(databaseName ?? Guid.NewGuid().ToString());
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(AppDbContext context)
    {
        // Add test users
        var users = new[]
        {
            new Models.User
            {
                Email = "test1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Models.User
            {
                Email = "test2@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password456"),
                FirstName = "Jane",
                LastName = "Smith",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Add test vehicles
        var vehicles = new[]
        {
            new Models.Vehicle
            {
                UserId = users[0].Id,
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                VIN = "1HGBH41JXMN109186",
                LicensePlate = "ABC123",
                CurrentMileage = 25000,
                Color = "Silver",
                PurchaseDate = DateTime.UtcNow.AddYears(-2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Models.Vehicle
            {
                UserId = users[0].Id,
                Make = "Honda",
                Model = "Accord",
                Year = 2019,
                VIN = "1HGBH41JXMN109187",
                LicensePlate = "XYZ789",
                CurrentMileage = 30000,
                Color = "Blue",
                PurchaseDate = DateTime.UtcNow.AddYears(-3),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Vehicles.AddRange(vehicles);
        context.SaveChanges();
    }
}
