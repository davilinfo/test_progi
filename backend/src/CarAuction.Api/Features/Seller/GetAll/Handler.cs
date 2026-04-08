using CarAuction.Api.Features.Seller.Create;
using CarAuction.Api.Features.Seller.Repository;

namespace CarAuction.Api.Features.Seller.GetAll;

public class GetAllSellersHandler : IRequestHandler<GetAllSellersQuery, IEnumerable<SellerResponse>>
{
    private readonly ISellerRepository _repository;
    public GetAllSellersHandler(ISellerRepository repository) => _repository = repository;

    public async Task<IEnumerable<SellerResponse>> Handle(GetAllSellersQuery request, CancellationToken cancellationToken)
    {
        var sellers = await _repository.GetAllAsync(cancellationToken);
        return sellers.Select(CreateSellerHandler.ToResponse);
    }
}
