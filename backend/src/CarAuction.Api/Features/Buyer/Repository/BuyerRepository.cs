using System.Text.RegularExpressions;
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

    public async Task<IEnumerable<Shared.Entity.Buyer>> GetAllAsync(
        int? age = null,
        string? name = null,
        string? email = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Buyers.AsNoTracking().AsQueryable();

        query = query.Where(b =>
             (!age.HasValue || b.Age == age.Value) &&
             (string.IsNullOrWhiteSpace(name) || Regex.IsMatch(b.Name, name, RegexOptions.IgnoreCase )) &&
             (string.IsNullOrWhiteSpace(email) || Regex.IsMatch(b.Email, email, RegexOptions.IgnoreCase ))
        );

        return await query.ToListAsync(cancellationToken);
    }

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
