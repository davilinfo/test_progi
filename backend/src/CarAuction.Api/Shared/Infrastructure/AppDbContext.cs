using Microsoft.EntityFrameworkCore;

namespace CarAuction.Api.Shared.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<BuyerFee> BuyerFees => Set<BuyerFee>();
    public DbSet<SellerFee> SellerFees => Set<SellerFee>();
    public DbSet<AssociationFee> AssociationFees => Set<AssociationFee>();
    public DbSet<StorageFee> StorageFees => Set<StorageFee>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasOne(a => a.Vehicle).WithMany().HasForeignKey(a => a.VehicleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Seller).WithMany().HasForeignKey(a => a.SellerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Buyer).WithMany().HasForeignKey(a => a.BuyerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vehicle>().Property(v => v.Type).HasConversion<int>();
        modelBuilder.Entity<Auction>().Property(a => a.Status).HasConversion<int>();
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<int>();
    }
}
