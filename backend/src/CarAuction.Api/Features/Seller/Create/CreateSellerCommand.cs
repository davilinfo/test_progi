namespace CarAuction.Api.Features.Seller.Create;

public record CreateSellerCommand(string Name, string Phone, string Email) : IRequest<SellerResponse>;

public record SellerResponse(Guid Id, string Name, string Phone, string Email);
