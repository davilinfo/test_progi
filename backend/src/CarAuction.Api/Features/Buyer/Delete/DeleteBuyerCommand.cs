namespace CarAuction.Api.Features.Buyer.Delete;

public record DeleteBuyerCommand(Guid Id) : IRequest<Unit>;
