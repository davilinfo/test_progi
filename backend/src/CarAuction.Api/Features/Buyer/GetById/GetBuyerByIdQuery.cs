using CarAuction.Api.Features.Buyer.Create;

namespace CarAuction.Api.Features.Buyer.GetById;

public record GetBuyerByIdQuery(Guid Id) : IRequest<BuyerResponse>;
