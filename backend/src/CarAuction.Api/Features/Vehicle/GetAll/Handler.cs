using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Repository;

namespace CarAuction.Api.Features.Vehicle.GetAll;

public class GetAllVehiclesHandler : IRequestHandler<GetAllVehiclesQuery, IEnumerable<VehicleResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetAllVehiclesHandler(IVehicleRepository repository) => _repository = repository;

    public async Task<IEnumerable<VehicleResponse>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await _repository.GetAllAsync(cancellationToken);
        return vehicles.Select(CreateVehicleHandler.ToResponse);
    }
}
