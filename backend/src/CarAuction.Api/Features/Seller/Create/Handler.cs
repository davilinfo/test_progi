using CarAuction.Api.Features.Seller.Repository;

namespace CarAuction.Api.Features.Seller.Create;

public class CreateSellerHandler : IRequestHandler<CreateSellerCommand, SellerResponse>
{
    private readonly ISellerRepository _repository;
    private readonly ILogger<CreateSellerHandler> _logger;

    public CreateSellerHandler(ISellerRepository repository, ILogger<CreateSellerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SellerResponse> Handle(CreateSellerCommand request, CancellationToken cancellationToken)
    {
        var seller = new Shared.Entity.Seller { Name = request.Name, Phone = request.Phone, Email = request.Email };
        var created = await _repository.CreateAsync(seller, cancellationToken);
        _logger.LogInformation("Seller {SellerId} created: {Email}", created.Id, created.Email);
        return ToResponse(created);
    }

    internal static SellerResponse ToResponse(Shared.Entity.Seller s) => new(s.Id, s.Name, s.Phone, s.Email);
}
