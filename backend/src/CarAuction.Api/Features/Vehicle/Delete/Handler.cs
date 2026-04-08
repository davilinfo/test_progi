using CarAuction.Api.Features.Vehicle.Repository;

namespace CarAuction.Api.Features.Vehicle.Delete;

public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, Unit>
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<DeleteVehicleHandler> _logger;

    public DeleteVehicleHandler(IVehicleRepository repository, ILogger<DeleteVehicleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var hasActiveAuction = await _repository.HasActiveAuctionAsync(request.Id, cancellationToken);
        if (hasActiveAuction)
            throw new DomainException("Cannot delete a vehicle with an active auction.");

        await _repository.DeleteAsync(request.Id, cancellationToken);
        _logger.LogInformation("Vehicle {VehicleId} deleted", request.Id);

        return Unit.Value;
    }
}
