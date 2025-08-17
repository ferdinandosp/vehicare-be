using Vehicare.API.Models;

namespace Vehicare.API.Models;

public class CreateVehiclePayload
{
    public Vehicle? Vehicle { get; set; }
    public string? Error { get; set; }
    public bool Success => Vehicle != null && string.IsNullOrEmpty(Error);
}

public class UpdateVehiclePayload
{
    public Vehicle? Vehicle { get; set; }
    public string? Error { get; set; }
    public bool Success => Vehicle != null && string.IsNullOrEmpty(Error);
}

public class DeleteVehiclePayload
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int? DeletedVehicleId { get; set; }
}
