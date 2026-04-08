using CarAuction.Api.Features.Buyer.Repository;

namespace CarAuction.Api.Features.Buyer.Create;

public class CreateBuyerHandler : IRequestHandler<CreateBuyerCommand, BuyerResponse>
{
    private readonly IBuyerRepository _repository;
    private readonly ILogger<CreateBuyerHandler> _logger;

    public CreateBuyerHandler(IBuyerRepository repository, ILogger<CreateBuyerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BuyerResponse> Handle(CreateBuyerCommand request, CancellationToken cancellationToken)
    {
        var buyer = new Shared.Entity.Buyer { Name = request.Name, Age = request.Age, Phone = request.Phone, Email = request.Email };
        var created = await _repository.CreateAsync(buyer, cancellationToken);
        _logger.LogInformation("Buyer {BuyerId} created: {Email}", created.Id, created.Email);
        return ToResponse(created);
    }

    internal static BuyerResponse ToResponse(Shared.Entity.Buyer b) => new(b.Id, b.Name, b.Age, b.Phone, b.Email);
}
