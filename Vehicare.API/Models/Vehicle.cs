namespace Vehicare.API.Models;

public class Vehicle
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}
