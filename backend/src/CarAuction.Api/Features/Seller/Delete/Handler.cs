using CarAuction.Api.Features.Seller.Repository;

namespace CarAuction.Api.Features.Seller.Delete;

public class DeleteSellerHandler : IRequestHandler<DeleteSellerCommand, Unit>
{
    private readonly ISellerRepository _repository;
    private readonly ILogger<DeleteSellerHandler> _logger;

    public DeleteSellerHandler(ISellerRepository repository, ILogger<DeleteSellerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteSellerCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        _logger.LogInformation("Seller {SellerId} deleted", request.Id);
        return Unit.Value;
    }
}
