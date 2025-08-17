using FluentAssertions;
using Vehicare.API.GraphQL.Inputs;
using Vehicare.API.Validators;
using Xunit;

namespace Vehicare.API.Tests.Validators;

public class VehicleInputValidatorsTests
{
    [Fact]
    public void CreateVehicleInputValidator_WithValidInput_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Make is required")]
    [InlineData("ThisIsAVeryLongMakeThatExceedsFiftyCharactersForSureX", "Make cannot exceed 50 characters")]
    public void CreateVehicleInputValidator_WithInvalidMake_ShouldFailValidation(string make, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = make,
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Make" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("", "Model is required")]
    [InlineData("ThisIsAVeryLongModelThatExceedsFiftyCharactersForSureX", "Model cannot exceed 50 characters")]
    public void CreateVehicleInputValidator_WithInvalidModel_ShouldFailValidation(string model, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = model,
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Model" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData(1900, "Year must be greater than 1900")]
    [InlineData(1899, "Year must be greater than 1900")]
    public void CreateVehicleInputValidator_WithYearTooOld_ShouldFailValidation(int year, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = year,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year" && e.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void CreateVehicleInputValidator_WithFutureYear_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var futureYear = DateTime.Now.Year + 2;
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = futureYear,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year" && e.ErrorMessage == "Year cannot be in the future");
    }

    [Theory]
    [InlineData("", "VIN is required")]
    [InlineData("1HGBH41JXMN10918", "VIN must be exactly 17 characters")] // 16 characters
    [InlineData("1HGBH41JXMN1091866", "VIN must be exactly 17 characters")] // 18 characters
    [InlineData("1HGBH41JXMN10918I", "VIN contains invalid characters")] // Contains 'I'
    [InlineData("1HGBH41JXMN10918O", "VIN contains invalid characters")] // Contains 'O'
    [InlineData("1HGBH41JXMN10918Q", "VIN contains invalid characters")] // Contains 'Q'
    [InlineData("1hgbh41jxmn109186", "VIN contains invalid characters")] // Lowercase
    public void CreateVehicleInputValidator_WithInvalidVIN_ShouldFailValidation(string vin, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = vin,
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "VIN" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("ThisIsAVeryLongLicensePlateThatExceedsTwentyCharacters", "License plate cannot exceed 20 characters")]
    public void CreateVehicleInputValidator_WithInvalidLicensePlate_ShouldFailValidation(string licensePlate, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = licensePlate,
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LicensePlate" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData(-1, "Current mileage cannot be negative")]
    [InlineData(-1000, "Current mileage cannot be negative")]
    public void CreateVehicleInputValidator_WithNegativeMileage_ShouldFailValidation(int mileage, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = mileage,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CurrentMileage" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("ThisIsAVeryLongColorNameThatExceedsThirtyCharacters", "Color cannot exceed 30 characters")]
    public void CreateVehicleInputValidator_WithInvalidColor_ShouldFailValidation(string color, string expectedMessage)
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            Color = color,
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Color" && e.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void CreateVehicleInputValidator_WithFuturePurchaseDate_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 25000,
            PurchaseDate = DateTime.Today.AddDays(1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PurchaseDate" && e.ErrorMessage == "Purchase date cannot be in the future");
    }

    [Fact]
    public void UpdateVehicleInputValidator_WithValidInput_ShouldPassValidation()
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = 1,
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "1HGBH41JXMN109186",
            LicensePlate = "ABC123",
            CurrentMileage = 25000,
            Color = "Silver",
            PurchaseDate = DateTime.Today.AddYears(-1)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, "Vehicle ID must be greater than 0")]
    [InlineData(-1, "Vehicle ID must be greater than 0")]
    public void UpdateVehicleInputValidator_WithInvalidId_ShouldFailValidation(int id, string expectedMessage)
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = id
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id" && e.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void UpdateVehicleInputValidator_WithNullOptionalFields_ShouldPassValidation()
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = 1,
            Make = null,
            Model = null,
            Year = null,
            VIN = null,
            LicensePlate = null,
            CurrentMileage = null,
            Color = null,
            PurchaseDate = null
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("ThisIsAVeryLongMakeThatExceedsFiftyCharactersForSureX", "Make cannot exceed 50 characters")]
    public void UpdateVehicleInputValidator_WithInvalidMake_ShouldFailValidation(string make, string expectedMessage)
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = 1,
            Make = make
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Make" && e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("1HGBH41JXMN10918", "VIN must be exactly 17 characters")]
    [InlineData("1HGBH41JXMN10918I", "VIN contains invalid characters")]
    public void UpdateVehicleInputValidator_WithInvalidVIN_ShouldFailValidation(string vin, string expectedMessage)
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = 1,
            VIN = vin
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "VIN" && e.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void UpdateVehicleInputValidator_WithEmptyStringFields_ShouldNotValidate()
    {
        // Arrange
        var validator = new UpdateVehicleInputValidator();
        var input = new UpdateVehicleInput
        {
            Id = 1,
            Make = "",
            Model = "",
            VIN = "",
            LicensePlate = "",
            Color = ""
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue(); // Empty strings should not trigger validation rules due to When conditions
    }

    [Fact]
    public void CreateVehicleInputValidator_WithValidVINFormats_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var validVINs = new[]
        {
            "1HGBH41JXMN109186",
            "WBAVB13596PT12345",
            "JM1BK32F681234567",
            "1G1ZT52F25F123456"
        };

        foreach (var vin in validVINs)
        {
            var input = new CreateVehicleInput
            {
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                VIN = vin,
                CurrentMileage = 25000,
                PurchaseDate = DateTime.Today.AddYears(-1)
            };

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue($"VIN {vin} should be valid");
        }
    }

    [Fact]
    public void CreateVehicleInputValidator_WithBoundaryValues_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = new string('A', 50), // Exactly 50 characters
            Model = new string('B', 50), // Exactly 50 characters
            Year = 1901, // Just above minimum
            VIN = "1HGBH41JXMN109186",
            LicensePlate = new string('C', 20), // Exactly 20 characters
            CurrentMileage = 0, // Minimum value
            Color = new string('D', 30), // Exactly 30 characters
            PurchaseDate = DateTime.Today // Today (boundary)
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateVehicleInputValidator_WithCurrentYear_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = DateTime.Now.Year,
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 100,
            PurchaseDate = DateTime.Today
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateVehicleInputValidator_WithNextYear_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateVehicleInputValidator();
        var input = new CreateVehicleInput
        {
            Make = "Toyota",
            Model = "Camry",
            Year = DateTime.Now.Year + 1, // Next year is allowed
            VIN = "1HGBH41JXMN109186",
            CurrentMileage = 0,
            PurchaseDate = DateTime.Today
        };

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
