namespace CarAuction.Api.Shared.Entity;

public class BuyerFee
{
    public Guid Id { get; set; }
    public decimal FeePercentage { get; set; }
    public decimal FeeCommonMin { get; set; }
    public decimal FeeCommonMax { get; set; }
    public decimal FeeLuxuryMin { get; set; }
    public decimal FeeLuxuryMax { get; set; }
}
