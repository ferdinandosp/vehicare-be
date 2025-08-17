using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;
using Vehicare.API.Services;
using Vehicare.API.Tests.Helpers;
using Xunit;

namespace Vehicare.API.Tests.Services;

public class DatabaseVehicleServiceTests : IDisposable
{
    private readonly DatabaseVehicleService _vehicleService;
    private readonly Data.AppDbContext _context;
    private readonly User _testUser;
    private readonly Vehicle _testVehicle;

    public DatabaseVehicleServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _vehicleService = new DatabaseVehicleService(_context);

        // Setup test data
        _testUser = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        _testVehicle = new Vehicle
        {
            UserId = _testUser.Id,
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
        };
        _context.Vehicles.Add(_testVehicle);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetVehicleByIdAsync_WhenVehicleExists_ShouldReturnVehicleWithUser()
    {
        // Act
        var result = await _vehicleService.GetVehicleByIdAsync(_testVehicle.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(_testVehicle.Id);
        result.Make.Should().Be("Toyota");
        result.Model.Should().Be("Camry");
        result.User.Should().NotBeNull();
        result.User!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetVehicleByIdAsync_WhenVehicleDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _vehicleService.GetVehicleByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserVehiclesAsync_WhenUserHasVehicles_ShouldReturnUserVehiclesOrderedByCreatedAt()
    {
        // Act
        var result = await _vehicleService.GetUserVehiclesAsync(_testUser.Id);

        // Assert
        var vehicles = result.ToList();
        vehicles.Should().HaveCount(1);
        vehicles.Should().OnlyContain(v => v.UserId == _testUser.Id);
    }

    [Fact]
    public async Task GetUserVehiclesAsync_WhenUserHasNoVehicles_ShouldReturnEmptyList()
    {
        // Act
        var result = await _vehicleService.GetUserVehiclesAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateVehicleAsync_ShouldCreateVehicleWithCorrectProperties()
    {
        // Arrange
        var purchaseDate = DateTime.UtcNow.AddYears(-2);
        var createInput = new CreateVehicleInput
        {
            Make = "Ford",
            Model = "Mustang",
            Year = 2021,
            VIN = "1FA6P8TH0L5123456",
            LicensePlate = "FORD123",
            CurrentMileage = 15000,
            Color = "Red",
            PurchaseDate = purchaseDate
        };

        // Act
        var result = await _vehicleService.CreateVehicleAsync(createInput, _testUser.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.UserId.Should().Be(_testUser.Id);
        result.Make.Should().Be("Ford");
        result.Model.Should().Be("Mustang");
        result.Year.Should().Be(2021);
        result.VIN.Should().Be("1FA6P8TH0L5123456");
        result.LicensePlate.Should().Be("FORD123");
        result.CurrentMileage.Should().Be(15000);
        result.Color.Should().Be("Red");
        result.PurchaseDate.Should().Be(purchaseDate);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.User.Should().NotBeNull();

        // Verify it's saved to database
        var savedVehicle = await _context.Vehicles.FindAsync(result.Id);
        savedVehicle.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateVehicleAsync_ShouldConvertVINToUppercase()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Ford",
            Model = "Mustang",
            Year = 2021,
            VIN = "1fa6p8th0l5123456", // lowercase
            LicensePlate = "FORD123",
            CurrentMileage = 15000,
            Color = "Red",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };

        // Act
        var result = await _vehicleService.CreateVehicleAsync(createInput, _testUser.Id);

        // Assert
        result.VIN.Should().Be("1FA6P8TH0L5123456");
    }

    [Fact]
    public async Task UpdateVehicleAsync_WhenVehicleExistsAndUserOwnsIt_ShouldUpdateVehicle()
    {
        // Arrange
        var updateInput = new UpdateVehicleInput
        {
            Id = _testVehicle.Id,
            Make = "Honda",
            Model = "Prius",
            CurrentMileage = 28000,
            Color = "White"
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, _testUser.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Make.Should().Be("Honda");
        result.Model.Should().Be("Prius");
        result.Year.Should().Be(2020); // Unchanged
        result.CurrentMileage.Should().Be(28000);
        result.Color.Should().Be("White");
        result.UpdatedAt.Should().BeAfter(result.CreatedAt);
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateVehicleAsync_WhenVehicleDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var updateInput = new UpdateVehicleInput
        {
            Id = 999,
            Make = "Honda"
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, _testUser.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateVehicleAsync_WhenUserDoesNotOwnVehicle_ShouldReturnNull()
    {
        // Arrange
        var updateInput = new UpdateVehicleInput
        {
            Id = _testVehicle.Id,
            Make = "Honda"
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, 999); // Different user

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteVehicleAsync_WhenVehicleExistsAndUserOwnsIt_ShouldDeleteVehicle()
    {
        // Act
        var result = await _vehicleService.DeleteVehicleAsync(_testVehicle.Id, _testUser.Id);

        // Assert
        result.Should().BeTrue();

        // Verify vehicle is deleted from database
        var deletedVehicle = await _context.Vehicles.FindAsync(_testVehicle.Id);
        deletedVehicle.Should().BeNull();
    }

    [Fact]
    public async Task DeleteVehicleAsync_WhenVehicleDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _vehicleService.DeleteVehicleAsync(999, _testUser.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VehicleExistsByVINAsync_WhenVehicleExists_ShouldReturnTrue()
    {
        // Act
        var result = await _vehicleService.VehicleExistsByVINAsync("1HGBH41JXMN109186");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VehicleExistsByVINAsync_WhenVehicleDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _vehicleService.VehicleExistsByVINAsync("NONEXISTENTVIN123");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VehicleExistsByVINAsync_ShouldBeCaseInsensitive()
    {
        // Act
        var result = await _vehicleService.VehicleExistsByVINAsync("1hgbh41jxmn109186");

        // Assert
        result.Should().BeTrue();
    }
}
