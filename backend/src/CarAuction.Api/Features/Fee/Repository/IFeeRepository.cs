namespace CarAuction.Api.Features.Fee.Repository;

public interface IFeeRepository
{
    Task<BuyerFee?> GetBuyerFeeAsync(CancellationToken cancellationToken = default);
    Task<SellerFee?> GetSellerFeeAsync(CancellationToken cancellationToken = default);
    Task<AssociationFee?> GetAssociationFeeByPriceAsync(decimal price, CancellationToken cancellationToken = default);
    Task<StorageFee?> GetStorageFeeAsync(CancellationToken cancellationToken = default);
}
