using CarAuction.Api.Features.Seller.Create;

namespace CarAuction.Api.Features.Seller.Update;

public record UpdateSellerCommand(Guid Id, string Name, string Phone, string Email) : IRequest<SellerResponse>;
