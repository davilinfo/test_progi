namespace CarAuction.Api.Features.Auction.Create;

public record CreateAuctionCommand(
    Guid SellerId,
    Guid VehicleId,
    decimal BasePrice,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<AuctionResponse>;

public record AuctionResponse(
    Guid Id,
    Guid SellerId,
    Guid? BuyerId,
    VehicleResponse Vehicle,
    string SellerName,
    decimal BasePrice,
    decimal Price,
    AuctionStatus Status,
    DateTime StartDate,
    DateTime EndDate
);

public record VehicleResponse(Guid Id, string Plate, VehicleType Type, int Year, string Model, string? Photo);
