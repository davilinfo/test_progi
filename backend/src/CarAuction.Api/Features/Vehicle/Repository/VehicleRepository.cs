using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Features.Vehicle.Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context) => _context = context;

    public async Task<Shared.Entity.Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<IEnumerable<Shared.Entity.Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Vehicles.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Shared.Entity.Vehicle> CreateAsync(Shared.Entity.Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        vehicle.Id = Guid.NewGuid();
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task<Shared.Entity.Vehicle> UpdateAsync(Shared.Entity.Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), id);
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasActiveAuctionAsync(Guid vehicleId, CancellationToken cancellationToken = default)
        => await _context.Auctions.AnyAsync(
            a => a.VehicleId == vehicleId && a.Status == AuctionStatus.Active, cancellationToken);
}
