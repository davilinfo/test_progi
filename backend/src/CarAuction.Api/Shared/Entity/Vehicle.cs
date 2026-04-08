namespace CarAuction.Api.Shared.Entity;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public VehicleType Type { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string? Photo { get; set; }
}
