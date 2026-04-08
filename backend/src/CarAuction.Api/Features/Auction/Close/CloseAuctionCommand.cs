using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Shared.Model;

namespace CarAuction.Api.Features.Auction.Close;

public record CloseAuctionCommand(Guid AuctionId) : IRequest<CloseAuctionResponse>;

public record CloseAuctionResponse(AuctionResponse Auction, FeeCalculationResult Fees);
