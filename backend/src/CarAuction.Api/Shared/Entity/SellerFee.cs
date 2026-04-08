namespace CarAuction.Api.Shared.Entity;

public class SellerFee
{
    public Guid Id { get; set; }
    public decimal FeeCommon { get; set; }
    public decimal FeeLuxury { get; set; }
}
