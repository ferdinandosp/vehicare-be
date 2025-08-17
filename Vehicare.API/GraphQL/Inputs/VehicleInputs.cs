namespace Vehicare.API.GraphQL.Inputs;

public class CreateVehicleInput
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
}

public class UpdateVehicleInput
{
    public int Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public int? CurrentMileage { get; set; }
    public string? Color { get; set; }
    public DateTime? PurchaseDate { get; set; }
}
