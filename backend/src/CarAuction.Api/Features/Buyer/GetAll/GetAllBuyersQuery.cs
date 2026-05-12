using CarAuction.Api.Features.Buyer.Create;

namespace CarAuction.Api.Features.Buyer.GetAll;

public record GetAllBuyersQuery(
    int? FilterAge = null,
    string? FilterName = null,
    string? FilterEmail = null
) : IRequest<IEnumerable<BuyerResponse>>;
