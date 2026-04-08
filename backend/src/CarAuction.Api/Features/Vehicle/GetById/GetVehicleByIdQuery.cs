using CarAuction.Api.Features.Vehicle.Create;

namespace CarAuction.Api.Features.Vehicle.GetById;

public record GetVehicleByIdQuery(Guid Id) : IRequest<VehicleResponse>;
