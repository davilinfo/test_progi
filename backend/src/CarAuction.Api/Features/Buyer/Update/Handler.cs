using CarAuction.Api.Features.Buyer.Create;
using CarAuction.Api.Features.Buyer.Repository;
using Buyer = CarAuction.Api.Shared.Entity.Buyer;

namespace CarAuction.Api.Features.Buyer.Update;

public class UpdateBuyerHandler : IRequestHandler<UpdateBuyerCommand, BuyerResponse>
{
    private readonly IBuyerRepository _repository;
    private readonly ILogger<UpdateBuyerHandler> _logger;

    public UpdateBuyerHandler(IBuyerRepository repository, ILogger<UpdateBuyerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BuyerResponse> Handle(UpdateBuyerCommand request, CancellationToken cancellationToken)
    {
        var buyer = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Buyer), request.Id);

        buyer.Name = request.Name;
        buyer.Age = request.Age;
        buyer.Phone = request.Phone;
        buyer.Email = request.Email;

        var updated = await _repository.UpdateAsync(buyer, cancellationToken);
        _logger.LogInformation("Buyer {BuyerId} updated", updated.Id);
        return CreateBuyerHandler.ToResponse(updated);
    }
}
