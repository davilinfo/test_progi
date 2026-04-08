using CarAuction.Api.Features.Auction.Repository;

namespace CarAuction.Api.Features.Auction.Create;

public class CreateAuctionHandler : IRequestHandler<CreateAuctionCommand, AuctionResponse>
{
    private readonly IAuctionRepository _repository;
    private readonly ILogger<CreateAuctionHandler> _logger;

    public CreateAuctionHandler(IAuctionRepository repository, ILogger<CreateAuctionHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AuctionResponse> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = new CarAuction.Api.Shared.Entity.Auction
        {
            SellerId = request.SellerId,
            VehicleId = request.VehicleId,
            BasePrice = request.BasePrice,
            Price = request.BasePrice,
            Status = AuctionStatus.Active,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var created = await _repository.CreateAsync(auction, cancellationToken);
        _logger.LogInformation("Auction {AuctionId} created for vehicle {VehicleId}", created.Id, created.VehicleId);

        return ToResponse(created);
    }

    internal static AuctionResponse ToResponse(CarAuction.Api.Shared.Entity.Auction a) => new(
        a.Id,
        a.SellerId,
        a.BuyerId,
        new VehicleResponse(a.Vehicle.Id, a.Vehicle.Plate, a.Vehicle.Type, a.Vehicle.Year, a.Vehicle.Model, a.Vehicle.Photo),
        a.Seller.Name,
        a.BasePrice,
        a.Price,
        a.Status,
        a.StartDate,
        a.EndDate
    );
}
