using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Repository;
using Vehicle = CarAuction.Api.Shared.Entity.Vehicle;

namespace CarAuction.Api.Features.Vehicle.GetById;

public class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdQuery, VehicleResponse>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleByIdHandler(IVehicleRepository repository) => _repository = repository;

    public async Task<VehicleResponse> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), request.Id);
        return CreateVehicleHandler.ToResponse(vehicle);
    }
}
