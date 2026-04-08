namespace CarAuction.Api.Features.Seller.Delete;

public record DeleteSellerCommand(Guid Id) : IRequest<Unit>;
