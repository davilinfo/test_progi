namespace CarAuction.Api.Features.Buyer.Repository;

public interface IBuyerRepository
{
    Task<Shared.Entity.Buyer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shared.Entity.Buyer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Shared.Entity.Buyer> CreateAsync(Shared.Entity.Buyer buyer, CancellationToken cancellationToken = default);
    Task<Shared.Entity.Buyer> UpdateAsync(Shared.Entity.Buyer buyer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
