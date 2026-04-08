using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Features.Auction.Repository;
using Microsoft.EntityFrameworkCore;
using Auction = CarAuction.Api.Shared.Entity.Auction;

namespace CarAuction.Api.Features.Auction.PlaceBid;

public class PlaceBidHandler : IRequestHandler<PlaceBidCommand, AuctionResponse>
{
    private readonly IAuctionRepository _repository;
    private readonly AppDbContext _context;
    private readonly ILogger<PlaceBidHandler> _logger;

    public PlaceBidHandler(IAuctionRepository repository, AppDbContext context, ILogger<PlaceBidHandler> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public async Task<AuctionResponse> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var auction = await _context.Auctions
            .Include(a => a.Vehicle)
            .Include(a => a.Seller)
            .Include(a => a.Buyer)
            .FirstOrDefaultAsync(a => a.Id == request.AuctionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Auction), request.AuctionId);

        if (auction.Status == AuctionStatus.Inactive)
            throw new DomainException("Cannot place a bid on an inactive auction.");

        if (DateTime.UtcNow > auction.EndDate)
        {
            auction.Status = AuctionStatus.Inactive;
            await _context.SaveChangesAsync(cancellationToken);
            throw new DomainException("This auction has already ended.");
        }

        if (request.BidAmount <= auction.Price)
            throw new DomainException($"Bid amount must be greater than the current price of {auction.Price:C}.");

        auction.Price = request.BidAmount;
        auction.BuyerId = request.BuyerId;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bid of {BidAmount} placed by buyer {BuyerId} on auction {AuctionId}", request.BidAmount, request.BuyerId, request.AuctionId);

        return CreateAuctionHandler.ToResponse(auction);
    }
}
