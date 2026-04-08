using CarAuction.Api.Features.Buyer.Create;
using CarAuction.Api.Features.Buyer.Repository;
using Buyer = CarAuction.Api.Shared.Entity.Buyer;

namespace CarAuction.Api.Features.Buyer.GetById;

public class GetBuyerByIdHandler : IRequestHandler<GetBuyerByIdQuery, BuyerResponse>
{
    private readonly IBuyerRepository _repository;
    public GetBuyerByIdHandler(IBuyerRepository repository) => _repository = repository;

    public async Task<BuyerResponse> Handle(GetBuyerByIdQuery request, CancellationToken cancellationToken)
    {
        var buyer = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Buyer), request.Id);
        return CreateBuyerHandler.ToResponse(buyer);
    }
}
