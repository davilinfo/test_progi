namespace CarAuction.Api.Shared.Model;

public class FeeCalculationResult
{
    public decimal BasePrice { get; set; }
    public VehicleType VehicleType { get; set; }
    public decimal BasicBuyerFee { get; set; }
    public decimal SpecialSellerFee { get; set; }
    public decimal AssociationFee { get; set; }
    public decimal StorageFee { get; set; }
    public decimal Total { get; set; }
}
