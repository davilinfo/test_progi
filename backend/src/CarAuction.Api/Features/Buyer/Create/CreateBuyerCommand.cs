namespace CarAuction.Api.Features.Buyer.Create;

public record CreateBuyerCommand(string Name, int Age, string Phone, string Email) : IRequest<BuyerResponse>;

public record BuyerResponse(Guid Id, string Name, int Age, string Phone, string Email);
