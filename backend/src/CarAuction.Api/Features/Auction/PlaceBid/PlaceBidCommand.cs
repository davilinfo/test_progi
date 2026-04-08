using CarAuction.Api.Features.Auction.Create;

namespace CarAuction.Api.Features.Auction.PlaceBid;

public record PlaceBidCommand(Guid AuctionId, Guid BuyerId, decimal BidAmount) : IRequest<AuctionResponse>;
