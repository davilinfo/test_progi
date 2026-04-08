using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Repository;
using Vehicle = CarAuction.Api.Shared.Entity.Vehicle;

namespace CarAuction.Api.Features.Vehicle.Update;

public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, VehicleResponse>
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<UpdateVehicleHandler> _logger;

    public UpdateVehicleHandler(IVehicleRepository repository, ILogger<UpdateVehicleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<VehicleResponse> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), request.Id);

        var hasActiveAuction = await _repository.HasActiveAuctionAsync(request.Id, cancellationToken);
        if (hasActiveAuction)
            throw new DomainException("Cannot update a vehicle with an active auction.");

        vehicle.Plate = request.Plate;
        vehicle.Type = request.Type;
        vehicle.Year = request.Year;
        vehicle.Model = request.Model;
        vehicle.Photo = request.Photo;

        var updated = await _repository.UpdateAsync(vehicle, cancellationToken);
        _logger.LogInformation("Vehicle {VehicleId} updated", updated.Id);

        return CreateVehicleHandler.ToResponse(updated);
    }
}
