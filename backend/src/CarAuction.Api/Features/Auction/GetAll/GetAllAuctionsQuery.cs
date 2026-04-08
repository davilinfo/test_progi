using CarAuction.Api.Features.Auction.Create;

namespace CarAuction.Api.Features.Auction.GetAll;

public record GetAllAuctionsQuery(AuctionStatus? Status) : IRequest<IEnumerable<AuctionResponse>>;
