using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Repository;
using Vehicle = CarAuction.Api.Shared.Entity.Vehicle;

namespace CarAuction.Api.Features.Vehicle.Create;

public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, VehicleResponse>
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<CreateVehicleHandler> _logger;

    public CreateVehicleHandler(IVehicleRepository repository, ILogger<CreateVehicleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<VehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Shared.Entity.Vehicle
        {
            Plate = request.Plate,
            Type = request.Type,
            Year = request.Year,
            Model = request.Model,
            Photo = request.Photo
        };

        var created = await _repository.CreateAsync(vehicle, cancellationToken);
        _logger.LogInformation("Vehicle {VehicleId} created: {Model}", created.Id, created.Model);

        return ToResponse(created);
    }

    internal static VehicleResponse ToResponse(Shared.Entity.Vehicle v) =>
        new(v.Id, v.Plate, v.Type, v.Year, v.Model, v.Photo);
}
