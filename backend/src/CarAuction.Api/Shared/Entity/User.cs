namespace CarAuction.Api.Shared.Entity;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? BuyerId { get; set; }
    public Guid? SellerId { get; set; }
}
