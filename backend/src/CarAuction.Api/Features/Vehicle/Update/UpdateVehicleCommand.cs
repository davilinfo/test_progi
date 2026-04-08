using CarAuction.Api.Features.Vehicle.Create;

namespace CarAuction.Api.Features.Vehicle.Update;

public record UpdateVehicleCommand(
    Guid Id,
    string Plate,
    VehicleType Type,
    int Year,
    string Model,
    string? Photo
) : IRequest<VehicleResponse>;
