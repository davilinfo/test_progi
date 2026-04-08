using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Features.Auction.Repository;

namespace CarAuction.Api.Features.Auction.GetAll;

public class GetAllAuctionsHandler : IRequestHandler<GetAllAuctionsQuery, IEnumerable<AuctionResponse>>
{
    private readonly IAuctionRepository _repository;

    public GetAllAuctionsHandler(IAuctionRepository repository) => _repository = repository;

    public async Task<IEnumerable<AuctionResponse>> Handle(GetAllAuctionsQuery request, CancellationToken cancellationToken)
    {
        var auctions = await _repository.GetAllAsync(request.Status, cancellationToken);
        return auctions.Select(CreateAuctionHandler.ToResponse);
    }
}
