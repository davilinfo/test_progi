using CarAuction.Api.Features.Vehicle.Create;

namespace CarAuction.Api.Features.Vehicle.GetAll;

public record GetAllVehiclesQuery : IRequest<IEnumerable<VehicleResponse>>;
