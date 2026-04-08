using CarAuction.Api.Features.Fee.Repository;
using CarAuction.Api.Shared.Services;

namespace CarAuction.Api.Features.Fee.Calculate;

public class CalculateFeeHandler : IRequestHandler<CalculateFeeQuery, FeeCalculationResult>
{
    private readonly IFeeRepository _feeRepository;
    private readonly ILogger<CalculateFeeHandler> _logger;

    public CalculateFeeHandler(IFeeRepository feeRepository, ILogger<CalculateFeeHandler> logger)
    {
        _feeRepository = feeRepository;
        _logger = logger;
    }

    public async Task<FeeCalculationResult> Handle(CalculateFeeQuery request, CancellationToken cancellationToken)
    {
        var buyerFee = await _feeRepository.GetBuyerFeeAsync(cancellationToken)
            ?? throw new DomainException("Buyer fee configuration not found.");

        var sellerFee = await _feeRepository.GetSellerFeeAsync(cancellationToken)
            ?? throw new DomainException("Seller fee configuration not found.");

        var associationFee = await _feeRepository.GetAssociationFeeByPriceAsync(request.BasePrice, cancellationToken)
            ?? throw new DomainException($"Association fee configuration not found for price {request.BasePrice}.");

        var storageFee = await _feeRepository.GetStorageFeeAsync(cancellationToken)
            ?? throw new DomainException("Storage fee configuration not found.");

        var result = FeeCalculationService.Calculate(
            request.BasePrice,
            request.VehicleType,
            buyerFee,
            sellerFee,
            associationFee,
            storageFee);

        _logger.LogInformation(
            "Fee calculated for price {Price} type {Type}: Total={Total}",
            request.BasePrice, request.VehicleType, result.Total);

        return result;
    }
}
