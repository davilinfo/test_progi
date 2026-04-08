using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Features.Auction.Repository;
using CarAuction.Api.Features.Fee.Repository;
using CarAuction.Api.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Auction = CarAuction.Api.Shared.Entity.Auction;

namespace CarAuction.Api.Features.Auction.Close;

public class CloseAuctionHandler : IRequestHandler<CloseAuctionCommand, CloseAuctionResponse>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IFeeRepository _feeRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<CloseAuctionHandler> _logger;

    public CloseAuctionHandler(
        IAuctionRepository auctionRepository,
        IFeeRepository feeRepository,
        AppDbContext context,
        ILogger<CloseAuctionHandler> logger)
    {
        _auctionRepository = auctionRepository;
        _feeRepository = feeRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<CloseAuctionResponse> Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await _context.Auctions
            .Include(a => a.Vehicle)
            .Include(a => a.Seller)
            .Include(a => a.Buyer)
            .FirstOrDefaultAsync(a => a.Id == request.AuctionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Auction), request.AuctionId);

        if (auction.Status == AuctionStatus.Inactive)
            throw new DomainException("Auction is already closed.");

        auction.Status = AuctionStatus.Inactive;
        await _context.SaveChangesAsync(cancellationToken);

        var buyerFee = await _feeRepository.GetBuyerFeeAsync(cancellationToken)
            ?? throw new DomainException("Buyer fee configuration not found.");
        var sellerFee = await _feeRepository.GetSellerFeeAsync(cancellationToken)
            ?? throw new DomainException("Seller fee configuration not found.");
        var associationFee = await _feeRepository.GetAssociationFeeByPriceAsync(auction.Price, cancellationToken)
            ?? throw new DomainException("Association fee configuration not found.");
        var storageFee = await _feeRepository.GetStorageFeeAsync(cancellationToken)
            ?? throw new DomainException("Storage fee configuration not found.");

        var fees = FeeCalculationService.Calculate(
            auction.Price, auction.Vehicle.Type, buyerFee, sellerFee, associationFee, storageFee);

        _logger.LogInformation("Auction {AuctionId} closed. Final bid: {Price}, Total with fees: {Total}",
            auction.Id, auction.Price, fees.Total);

        return new CloseAuctionResponse(CreateAuctionHandler.ToResponse(auction), fees);
    }
}
