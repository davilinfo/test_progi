namespace CarAuction.Api.Shared.Entity;

public class Auction
{
    public Guid Id { get; set; }
    public Guid? BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid VehicleId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal Price { get; set; }
    public AuctionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Buyer? Buyer { get; set; }
    public Seller Seller { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
