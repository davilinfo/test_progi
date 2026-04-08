using CarAuction.Api.Features.Buyer.Create;

namespace CarAuction.Api.Features.Buyer.GetAll;

public record GetAllBuyersQuery : IRequest<IEnumerable<BuyerResponse>>;
