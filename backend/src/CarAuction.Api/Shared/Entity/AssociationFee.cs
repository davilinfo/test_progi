namespace CarAuction.Api.Shared.Entity;

public class AssociationFee
{
    public Guid Id { get; set; }
    public decimal Fee { get; set; }
    public decimal MinRange { get; set; }
    public decimal MaxRange { get; set; }
}
