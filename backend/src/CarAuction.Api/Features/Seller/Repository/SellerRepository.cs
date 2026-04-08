using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Features.Seller.Repository;

public class SellerRepository : ISellerRepository
{
    private readonly AppDbContext _context;
    public SellerRepository(AppDbContext context) => _context = context;

    public async Task<Shared.Entity.Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IEnumerable<Shared.Entity.Seller>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Sellers.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Shared.Entity.Seller> CreateAsync(Shared.Entity.Seller seller, CancellationToken cancellationToken = default)
    {
        seller.Id = Guid.NewGuid();
        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync(cancellationToken);
        return seller;
    }

    public async Task<Shared.Entity.Seller> UpdateAsync(Shared.Entity.Seller seller, CancellationToken cancellationToken = default)
    {
        _context.Sellers.Update(seller);
        await _context.SaveChangesAsync(cancellationToken);
        return seller;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var seller = await _context.Sellers.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Seller), id);
        _context.Sellers.Remove(seller);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
