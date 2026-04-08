using CarAuction.Api.Features.Buyer.Create;
using CarAuction.Api.Features.Buyer.Repository;

namespace CarAuction.Api.Features.Buyer.GetAll;

public class GetAllBuyersHandler : IRequestHandler<GetAllBuyersQuery, IEnumerable<BuyerResponse>>
{
    private readonly IBuyerRepository _repository;
    public GetAllBuyersHandler(IBuyerRepository repository) => _repository = repository;

    public async Task<IEnumerable<BuyerResponse>> Handle(GetAllBuyersQuery request, CancellationToken cancellationToken)
    {
        var buyers = await _repository.GetAllAsync(cancellationToken);
        return buyers.Select(CreateBuyerHandler.ToResponse);
    }
}
