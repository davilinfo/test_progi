using CarAuction.Api.Features.Seller.Create;
using CarAuction.Api.Features.Seller.Repository;
using Seller = CarAuction.Api.Shared.Entity.Seller;

namespace CarAuction.Api.Features.Seller.Update;

public class UpdateSellerHandler : IRequestHandler<UpdateSellerCommand, SellerResponse>
{
    private readonly ISellerRepository _repository;
    private readonly ILogger<UpdateSellerHandler> _logger;

    public UpdateSellerHandler(ISellerRepository repository, ILogger<UpdateSellerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SellerResponse> Handle(UpdateSellerCommand request, CancellationToken cancellationToken)
    {
        var seller = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Seller), request.Id);

        seller.Name = request.Name;
        seller.Phone = request.Phone;
        seller.Email = request.Email;

        var updated = await _repository.UpdateAsync(seller, cancellationToken);
        _logger.LogInformation("Seller {SellerId} updated", updated.Id);
        return CreateSellerHandler.ToResponse(updated);
    }
}
