namespace CarAuction.Api.Features.Seller.Repository;

public interface ISellerRepository
{
    Task<Shared.Entity.Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shared.Entity.Seller>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Shared.Entity.Seller> CreateAsync(Shared.Entity.Seller seller, CancellationToken cancellationToken = default);
    Task<Shared.Entity.Seller> UpdateAsync(Shared.Entity.Seller seller, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
