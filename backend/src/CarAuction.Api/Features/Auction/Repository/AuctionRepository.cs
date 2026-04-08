using CarAuction.Api.Features.Auction.Repository;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Features.Auction.Repository;

public class AuctionRepository : IAuctionRepository
{
    private readonly AppDbContext _context;

    public AuctionRepository(AppDbContext context) => _context = context;

    public async Task<CarAuction.Api.Shared.Entity.Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Auctions
            .AsNoTracking()
            .Include(a => a.Vehicle)
            .Include(a => a.Seller)
            .Include(a => a.Buyer)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<CarAuction.Api.Shared.Entity.Auction>> GetAllAsync(AuctionStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Auctions
            .AsNoTracking()
            .Include(a => a.Vehicle)
            .Include(a => a.Seller)
            .Include(a => a.Buyer)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Shared.Entity.Auction> CreateAsync(Shared.Entity.Auction auction, CancellationToken cancellationToken = default)
    {
        auction.Id = Guid.NewGuid();
        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(auction.Id, cancellationToken))!;
    }

    public async Task<Shared.Entity.Auction> UpdateAsync(Shared.Entity.Auction auction, CancellationToken cancellationToken = default)
    {
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(auction.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auction = await _context.Auctions.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Auction), id);
        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
