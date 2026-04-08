namespace CarAuction.Api.Features.Vehicle.Repository;

public interface IVehicleRepository
{
    Task<Shared.Entity.Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shared.Entity.Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Shared.Entity.Vehicle> CreateAsync(Shared.Entity.Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<Shared.Entity.Vehicle> UpdateAsync(Shared.Entity.Vehicle vehicle, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveAuctionAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}
