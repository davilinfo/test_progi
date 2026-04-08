using CarAuction.Api.Features.Buyer.Create;

namespace CarAuction.Api.Features.Buyer.Update;

public record UpdateBuyerCommand(Guid Id, string Name, int Age, string Phone, string Email) : IRequest<BuyerResponse>;
