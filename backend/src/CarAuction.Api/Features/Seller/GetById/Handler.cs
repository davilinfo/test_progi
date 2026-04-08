using CarAuction.Api.Features.Seller.Create;
using CarAuction.Api.Features.Seller.Repository;
using Seller = CarAuction.Api.Shared.Entity.Seller;

namespace CarAuction.Api.Features.Seller.GetById;

public class GetSellerByIdHandler : IRequestHandler<GetSellerByIdQuery, SellerResponse>
{
    private readonly ISellerRepository _repository;
    public GetSellerByIdHandler(ISellerRepository repository) => _repository = repository;

    public async Task<SellerResponse> Handle(GetSellerByIdQuery request, CancellationToken cancellationToken)
    {
        var seller = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Seller), request.Id);
        return CreateSellerHandler.ToResponse(seller);
    }
}
