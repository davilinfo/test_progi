using CarAuction.Api.Features.Buyer.Repository;

namespace CarAuction.Api.Features.Buyer.Delete;

public class DeleteBuyerHandler : IRequestHandler<DeleteBuyerCommand, Unit>
{
    private readonly IBuyerRepository _repository;
    private readonly ILogger<DeleteBuyerHandler> _logger;

    public DeleteBuyerHandler(IBuyerRepository repository, ILogger<DeleteBuyerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteBuyerCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        _logger.LogInformation("Buyer {BuyerId} deleted", request.Id);
        return Unit.Value;
    }
}
