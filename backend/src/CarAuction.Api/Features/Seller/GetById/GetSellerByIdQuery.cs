using CarAuction.Api.Features.Seller.Create;

namespace CarAuction.Api.Features.Seller.GetById;

public record GetSellerByIdQuery(Guid Id) : IRequest<SellerResponse>;
