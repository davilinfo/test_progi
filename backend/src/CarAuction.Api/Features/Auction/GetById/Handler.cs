using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Features.Auction.Repository;

namespace CarAuction.Api.Features.Auction.GetById;

public class GetAuctionByIdHandler : IRequestHandler<GetAuctionByIdQuery, AuctionResponse>
{
    private readonly IAuctionRepository _repository;

    public GetAuctionByIdHandler(IAuctionRepository repository) => _repository = repository;

    public async Task<AuctionResponse> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
    {
        var auction = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Auction), request.Id);
        return CreateAuctionHandler.ToResponse(auction);
    }
}
