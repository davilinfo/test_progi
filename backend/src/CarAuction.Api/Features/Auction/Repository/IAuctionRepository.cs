namespace CarAuction.Api.Features.Auction.Repository;

public interface IAuctionRepository
{
    Task<Shared.Entity.Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shared.Entity.Auction>> GetAllAsync(AuctionStatus? status, CancellationToken cancellationToken = default);
    Task<Shared.Entity.Auction> CreateAsync(Shared.Entity.Auction auction, CancellationToken cancellationToken = default);
    Task<Shared.Entity.Auction> UpdateAsync(Shared.Entity.Auction auction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
