namespace CarAuction.Api.Features.Vehicle.Create;

public record CreateVehicleCommand(
    string Plate,
    VehicleType Type,
    int Year,
    string Model,
    string? Photo
) : IRequest<VehicleResponse>;

public record VehicleResponse(Guid Id, string Plate, VehicleType Type, int Year, string Model, string? Photo);
