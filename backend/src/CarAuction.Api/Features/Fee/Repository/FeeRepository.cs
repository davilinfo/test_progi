using CarAuction.Api.Features.Fee.Repository;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Features.Fee.Repository;

public class FeeRepository : IFeeRepository
{
    private readonly AppDbContext _context;

    public FeeRepository(AppDbContext context) => _context = context;

    public async Task<BuyerFee?> GetBuyerFeeAsync(CancellationToken cancellationToken = default)
        => await _context.BuyerFees.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<SellerFee?> GetSellerFeeAsync(CancellationToken cancellationToken = default)
        => await _context.SellerFees.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<AssociationFee?> GetAssociationFeeByPriceAsync(decimal price, CancellationToken cancellationToken = default)
        => await _context.AssociationFees
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.MinRange <= price && f.MaxRange >= price, cancellationToken);

    public async Task<StorageFee?> GetStorageFeeAsync(CancellationToken cancellationToken = default)
        => await _context.StorageFees.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
}
