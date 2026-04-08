using CarAuction.Api.Features.Seller.Create;

namespace CarAuction.Api.Features.Seller.GetAll;

public record GetAllSellersQuery : IRequest<IEnumerable<SellerResponse>>;
