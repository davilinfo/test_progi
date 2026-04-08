using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Shared.Infrastructure;

public static class DataSeeder
{
    private static readonly Guid BuyerFeeId = new("00000000-0000-0000-0004-000000000001");
    private static readonly Guid SellerFeeId = new("00000000-0000-0000-0004-000000000002");
    private static readonly Guid AssocFee1Id = new("00000000-0000-0000-0004-000000000003");
    private static readonly Guid AssocFee2Id = new("00000000-0000-0000-0004-000000000004");
    private static readonly Guid AssocFee3Id = new("00000000-0000-0000-0004-000000000005");
    private static readonly Guid AssocFee4Id = new("00000000-0000-0000-0004-000000000006");
    private static readonly Guid StorageFeeId = new("00000000-0000-0000-0004-000000000007");

    private static readonly Guid Seller1Id = new("00000000-0000-0000-0001-000000000001");
    private static readonly Guid Seller2Id = new("00000000-0000-0000-0001-000000000002");

    private static readonly Guid Buyer1Id = new("00000000-0000-0000-0002-000000000001");
    private static readonly Guid Buyer2Id = new("00000000-0000-0000-0002-000000000002");

    private static readonly Guid Vehicle1Id = new("00000000-0000-0000-0003-000000000001");
    private static readonly Guid Vehicle2Id = new("00000000-0000-0000-0003-000000000002");
    private static readonly Guid Vehicle3Id = new("00000000-0000-0000-0003-000000000003");
    private static readonly Guid Vehicle4Id = new("00000000-0000-0000-0003-000000000004");
    private static readonly Guid Vehicle5Id = new("00000000-0000-0000-0003-000000000005");

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        SeedFees(context);
        SeedSellers(context);
        SeedBuyers(context);
        SeedVehicles(context);
        SeedUsers(context);
        SeedAuctions(context);

        await context.SaveChangesAsync();
    }

    private static void SeedFees(AppDbContext context)
    {
        context.BuyerFees.Add(new BuyerFee
        {
            Id = BuyerFeeId,
            FeePercentage = 0.10m,
            FeeCommonMin = 10m,
            FeeCommonMax = 50m,
            FeeLuxuryMin = 25m,
            FeeLuxuryMax = 200m
        });

        context.SellerFees.Add(new SellerFee
        {
            Id = SellerFeeId,
            FeeCommon = 0.02m,
            FeeLuxury = 0.04m
        });

        context.AssociationFees.AddRange(
            new AssociationFee { Id = AssocFee1Id, Fee = 5m, MinRange = 1m, MaxRange = 500m },
            new AssociationFee { Id = AssocFee2Id, Fee = 10m, MinRange = 500.01m, MaxRange = 1000m },
            new AssociationFee { Id = AssocFee3Id, Fee = 15m, MinRange = 1000.01m, MaxRange = 3000m },
            new AssociationFee { Id = AssocFee4Id, Fee = 20m, MinRange = 3000.01m, MaxRange = 9_999_999m }
        );

        context.StorageFees.Add(new StorageFee { Id = StorageFeeId, Fee = 100m });
    }

    private static void SeedSellers(AppDbContext context)
    {
        context.Sellers.AddRange(
            new Seller { Id = Seller1Id, Name = "AutoDeal Inc.", Phone = "555-0101", Email = "seller1@carauction.com" },
            new Seller { Id = Seller2Id, Name = "Premium Cars LLC", Phone = "555-0102", Email = "seller2@carauction.com" }
        );
    }

    private static void SeedBuyers(AppDbContext context)
    {
        context.Buyers.AddRange(
            new Buyer { Id = Buyer1Id, Name = "Alice Johnson", Age = 34, Phone = "555-0201", Email = "buyer1@carauction.com" },
            new Buyer { Id = Buyer2Id, Name = "Bob Williams", Age = 42, Phone = "555-0202", Email = "buyer2@carauction.com" }
        );
    }

    private static void SeedVehicles(AppDbContext context)
    {
        context.Vehicles.AddRange(
            new Vehicle { Id = Vehicle1Id, Plate = "ABC-001", Type = VehicleType.Common, Year = 2019, Model = "Toyota Corolla" },
            new Vehicle { Id = Vehicle2Id, Plate = "XYZ-002", Type = VehicleType.Luxury, Year = 2022, Model = "BMW 7 Series" },
            new Vehicle { Id = Vehicle3Id, Plate = "DEF-003", Type = VehicleType.Common, Year = 2020, Model = "Honda Civic" },
            new Vehicle { Id = Vehicle4Id, Plate = "GHI-004", Type = VehicleType.Luxury, Year = 2021, Model = "Mercedes S-Class" },
            new Vehicle { Id = Vehicle5Id, Plate = "JKL-005", Type = VehicleType.Common, Year = 2018, Model = "Ford Focus" }
        );
    }

    private static void SeedUsers(AppDbContext context)
    {
        var hash = HashPassword("Password123!");
        context.Users.AddRange(
            new User { Id = new Guid("00000000-0000-0000-0000-000000000001"), Email = "admin@carauction.com", PasswordHash = hash, Role = UserRole.Admin },
            new User { Id = new Guid("00000000-0000-0000-0000-000000000002"), Email = "seller1@carauction.com", PasswordHash = hash, Role = UserRole.Seller, SellerId = Seller1Id },
            new User { Id = new Guid("00000000-0000-0000-0000-000000000003"), Email = "seller2@carauction.com", PasswordHash = hash, Role = UserRole.Seller, SellerId = Seller2Id },
            new User { Id = new Guid("00000000-0000-0000-0000-000000000004"), Email = "buyer1@carauction.com", PasswordHash = hash, Role = UserRole.Buyer, BuyerId = Buyer1Id },
            new User { Id = new Guid("00000000-0000-0000-0000-000000000005"), Email = "buyer2@carauction.com", PasswordHash = hash, Role = UserRole.Buyer, BuyerId = Buyer2Id }
        );
    }

    private static void SeedAuctions(AppDbContext context)
    {
        var now = DateTime.UtcNow;
        context.Auctions.AddRange(
            new Auction
            {
                Id = new Guid("00000000-0000-0000-0005-000000000001"),
                SellerId = Seller1Id, VehicleId = Vehicle1Id,
                BasePrice = 8000m, Price = 8000m,
                Status = AuctionStatus.Active,
                StartDate = now.AddDays(-1), EndDate = now.AddDays(5)
            },
            new Auction
            {
                Id = new Guid("00000000-0000-0000-0005-000000000002"),
                SellerId = Seller2Id, VehicleId = Vehicle2Id,
                BasePrice = 45000m, Price = 45000m,
                Status = AuctionStatus.Active,
                StartDate = now.AddDays(-2), EndDate = now.AddDays(3)
            },
            new Auction
            {
                Id = new Guid("00000000-0000-0000-0005-000000000003"),
                SellerId = Seller1Id, VehicleId = Vehicle3Id,
                BasePrice = 6500m, Price = 6500m,
                Status = AuctionStatus.Active,
                StartDate = now, EndDate = now.AddDays(7)
            },
            new Auction
            {
                Id = new Guid("00000000-0000-0000-0005-000000000004"),
                SellerId = Seller2Id, VehicleId = Vehicle4Id,
                BuyerId = Buyer1Id,
                BasePrice = 60000m, Price = 62000m,
                Status = AuctionStatus.Inactive,
                StartDate = now.AddDays(-10), EndDate = now.AddDays(-1)
            },
            new Auction
            {
                Id = new Guid("00000000-0000-0000-0005-000000000005"),
                SellerId = Seller1Id, VehicleId = Vehicle5Id,
                BuyerId = Buyer2Id,
                BasePrice = 4000m, Price = 4200m,
                Status = AuctionStatus.Inactive,
                StartDate = now.AddDays(-15), EndDate = now.AddDays(-5)
            }
        );
    }

    public static string HashPassword(string password)
    {
        // NOTE: Use bcrypt or Argon2 in production
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + "CarAuctionSalt2024");
        return Convert.ToBase64String(sha256.ComputeHash(bytes));
    }
}
