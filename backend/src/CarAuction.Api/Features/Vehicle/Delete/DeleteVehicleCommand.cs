namespace CarAuction.Api.Features.Vehicle.Delete;

public record DeleteVehicleCommand(Guid Id) : IRequest<Unit>;
