using CarAuction.Api.Features.Auction.Create;

namespace CarAuction.Api.Features.Auction.GetById;

public record GetAuctionByIdQuery(Guid Id) : IRequest<AuctionResponse>;
