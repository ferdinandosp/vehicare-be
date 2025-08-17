using FluentAssertions;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Models;
using Vehicare.API.Services;
using Xunit;

namespace Vehicare.API.Tests.Services;

public class VehicleServiceTests
{
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        _vehicleService = new VehicleService();
    }

    [Fact]
    public async Task GetVehicleByIdAsync_WhenVehicleExists_ShouldReturnVehicle()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, 1);

        // Act
        var result = await _vehicleService.GetVehicleByIdAsync(createdVehicle.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdVehicle.Id);
        result.Make.Should().Be("Toyota");
        result.Model.Should().Be("Camry");
        result.VIN.Should().Be("1HGBH41JXMN109186");
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
    public async Task GetUserVehiclesAsync_WhenUserHasVehicles_ShouldReturnUserVehicles()
    {
        // Arrange
        var userId = 1;
        var createInput1 = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createInput2 = new CreateVehicleInput
        {
            Make = "Honda",
            Model = "Accord",
            Year = 2019,
            VIN = "1HGBH41JXMN109187",
            LicensePlate = "XYZ789",
            CurrentMileage = 30000,
            Color = "Blue",
            PurchaseDate = DateTime.UtcNow.AddYears(-3)
        };

        await _vehicleService.CreateVehicleAsync(createInput1, userId);
        await _vehicleService.CreateVehicleAsync(createInput2, userId);
        await _vehicleService.CreateVehicleAsync(createInput1, 2); // Different user

        // Act
        var result = await _vehicleService.GetUserVehiclesAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(v => v.UserId == userId);
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
        var userId = 1;
        var purchaseDate = DateTime.UtcNow.AddYears(-2);
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = purchaseDate
        };

        // Act
        var result = await _vehicleService.CreateVehicleAsync(createInput, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.UserId.Should().Be(userId);
        result.Make.Should().Be("Toyota");
        result.Model.Should().Be("Camry");
        result.Year.Should().Be(2020);
        result.VIN.Should().Be("1HGBH41JXMN109186");
        result.LicensePlate.Should().Be("ABC123");
        result.CurrentMileage.Should().Be(25000);
        result.Color.Should().Be("Silver");
        result.PurchaseDate.Should().Be(purchaseDate);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task CreateVehicleAsync_ShouldAssignUniqueIds()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };

        // Act
        var vehicle1 = await _vehicleService.CreateVehicleAsync(createInput, 1);
        createInput.VIN = "1HGBH41JXMN109187"; // Different VIN
        var vehicle2 = await _vehicleService.CreateVehicleAsync(createInput, 1);

        // Assert
        vehicle1.Id.Should().NotBe(vehicle2.Id);
        vehicle1.Id.Should().BeGreaterThan(0);
        vehicle2.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateVehicleAsync_WhenVehicleExistsAndUserOwnsIt_ShouldUpdateVehicle()
    {
        // Arrange
        var userId = 1;
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, userId);

        var updateInput = new UpdateVehicleInput
        {
            Id = createdVehicle.Id,
            Make = "Honda",
            Model = "Accord",
            CurrentMileage = 30000,
            Color = "Blue"
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Make.Should().Be("Honda");
        result.Model.Should().Be("Accord");
        result.Year.Should().Be(2020); // Unchanged
        result.VIN.Should().Be("1HGBH41JXMN109186"); // Unchanged
        result.CurrentMileage.Should().Be(30000);
        result.Color.Should().Be("Blue");
        result.UpdatedAt.Should().BeAfter(result.CreatedAt);
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
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateVehicleAsync_WhenUserDoesNotOwnVehicle_ShouldReturnNull()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, 1);

        var updateInput = new UpdateVehicleInput
        {
            Id = createdVehicle.Id,
            Make = "Honda"
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, 2); // Different user

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateVehicleAsync_WithNullValues_ShouldNotUpdateThoseFields()
    {
        // Arrange
        var userId = 1;
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, userId);

        var updateInput = new UpdateVehicleInput
        {
            Id = createdVehicle.Id,
            Make = "Honda",
            // All other fields are null
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(updateInput, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Make.Should().Be("Honda"); // Updated
        result.Model.Should().Be("Camry"); // Unchanged
        result.Year.Should().Be(2020); // Unchanged
        result.VIN.Should().Be("1HGBH41JXMN109186"); // Unchanged
        result.CurrentMileage.Should().Be(25000); // Unchanged
        result.Color.Should().Be("Silver"); // Unchanged
    }

    [Fact]
    public async Task DeleteVehicleAsync_WhenVehicleExistsAndUserOwnsIt_ShouldDeleteVehicle()
    {
        // Arrange
        var userId = 1;
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, userId);

        // Act
        var result = await _vehicleService.DeleteVehicleAsync(createdVehicle.Id, userId);

        // Assert
        result.Should().BeTrue();

        // Verify vehicle is deleted
        var deletedVehicle = await _vehicleService.GetVehicleByIdAsync(createdVehicle.Id);
        deletedVehicle.Should().BeNull();
    }

    [Fact]
    public async Task DeleteVehicleAsync_WhenVehicleDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _vehicleService.DeleteVehicleAsync(999, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteVehicleAsync_WhenUserDoesNotOwnVehicle_ShouldReturnFalse()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        var createdVehicle = await _vehicleService.CreateVehicleAsync(createInput, 1);

        // Act
        var result = await _vehicleService.DeleteVehicleAsync(createdVehicle.Id, 2); // Different user

        // Assert
        result.Should().BeFalse();

        // Verify vehicle still exists
        var vehicle = await _vehicleService.GetVehicleByIdAsync(createdVehicle.Id);
        vehicle.Should().NotBeNull();
    }

    [Fact]
    public async Task VehicleExistsByVINAsync_WhenVehicleExists_ShouldReturnTrue()
    {
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        await _vehicleService.CreateVehicleAsync(createInput, 1);

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
        // Arrange
        var createInput = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.UtcNow.AddYears(-2)
        };
        await _vehicleService.CreateVehicleAsync(createInput, 1);

        // Act
        var result = await _vehicleService.VehicleExistsByVINAsync("1hgbh41jxmn109186");

        // Assert
        result.Should().BeTrue();
    }
}
