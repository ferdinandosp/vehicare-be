using FluentValidation;
using Vehicare.API.GraphQL.Inputs;

namespace Vehicare.API.Validators;

public class CreateVehicleInputValidator : AbstractValidator<CreateVehicleInput>
{
    public CreateVehicleInputValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Make is required")
            .MaximumLength(50)
            .WithMessage("Make cannot exceed 50 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .MaximumLength(50)
            .WithMessage("Model cannot exceed 50 characters");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage("Year cannot be in the future");

        RuleFor(x => x.VIN)
            .NotEmpty()
            .WithMessage("VIN is required")
            .Length(17)
            .WithMessage("VIN must be exactly 17 characters")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$")
            .WithMessage("VIN contains invalid characters");

        RuleFor(x => x.LicensePlate)
            .MaximumLength(20)
            .WithMessage("License plate cannot exceed 20 characters");

        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current mileage cannot be negative");

        RuleFor(x => x.Color)
            .MaximumLength(30)
            .WithMessage("Color cannot exceed 30 characters");

        RuleFor(x => x.PurchaseDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Purchase date cannot be in the future");
    }
}

public class UpdateVehicleInputValidator : AbstractValidator<UpdateVehicleInput>
{
    public UpdateVehicleInputValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be greater than 0");

        RuleFor(x => x.Make)
            .MaximumLength(50)
            .WithMessage("Make cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Make));

        RuleFor(x => x.Model)
            .MaximumLength(50)
            .WithMessage("Model cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Model));

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage("Year cannot be in the future")
            .When(x => x.Year.HasValue);

        RuleFor(x => x.VIN)
            .Length(17)
            .WithMessage("VIN must be exactly 17 characters")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$")
            .WithMessage("VIN contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.VIN));

        RuleFor(x => x.LicensePlate)
            .MaximumLength(20)
            .WithMessage("License plate cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.LicensePlate));

        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current mileage cannot be negative")
            .When(x => x.CurrentMileage.HasValue);

        RuleFor(x => x.Color)
            .MaximumLength(30)
            .WithMessage("Color cannot exceed 30 characters")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.PurchaseDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Purchase date cannot be in the future")
            .When(x => x.PurchaseDate.HasValue);
    }
}
