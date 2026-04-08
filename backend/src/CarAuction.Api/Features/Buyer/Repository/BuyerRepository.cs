using CarAuction.Api.Features.Buyer.Repository;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Features.Buyer.Repository;

public class BuyerRepository : IBuyerRepository
{
    private readonly AppDbContext _context;

    public BuyerRepository(AppDbContext context) => _context = context;

    public async Task<Shared.Entity.Buyer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Buyers.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IEnumerable<Shared.Entity.Buyer>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Buyers.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Shared.Entity.Buyer> CreateAsync(Shared.Entity.Buyer buyer, CancellationToken cancellationToken = default)
    {
        buyer.Id = Guid.NewGuid();
        _context.Buyers.Add(buyer);
        await _context.SaveChangesAsync(cancellationToken);
        return buyer;
    }

    public async Task<Shared.Entity.Buyer> UpdateAsync(Shared.Entity.Buyer buyer, CancellationToken cancellationToken = default)
    {
        _context.Buyers.Update(buyer);
        await _context.SaveChangesAsync(cancellationToken);
        return buyer;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var buyer = await _context.Buyers.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Buyer), id);
        _context.Buyers.Remove(buyer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
