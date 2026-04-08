using CarAuction.Api.Features.Auction.PlaceBid;
using CarAuction.Api.Features.Auction.Repository;
using CarAuction.Api.Shared.Entity;
using CarAuction.Api.Shared.Enums;
using CarAuction.Api.Shared.Exceptions;
using CarAuction.Api.Shared.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Tests.Features.Auction;

public class PlaceBidHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IAuctionRepository> _repoMock;
    private readonly Mock<ILogger<PlaceBidHandler>> _loggerMock;
    private readonly PlaceBidHandler _handler;

    private readonly Guid _auctionId = Guid.NewGuid();
    private readonly Guid _buyerId = Guid.NewGuid();
    private readonly Guid _sellerId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();

    public PlaceBidHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"PlaceBidTest_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);

        _repoMock = new Mock<IAuctionRepository>();
        _loggerMock = new Mock<ILogger<PlaceBidHandler>>();
        _handler = new PlaceBidHandler(_repoMock.Object, _context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Sellers.Add(new Seller { Id = _sellerId, Name = "Test Seller", Phone = "555-0001", Email = "seller@test.com" });
        _context.Buyers.Add(new Buyer { Id = _buyerId, Name = "Test Buyer", Age = 30, Phone = "555-0002", Email = "buyer@test.com" });
        _context.Vehicles.Add(new Api.Shared.Entity.Vehicle
        {
            Id = _vehicleId, Plate = "TST-001", Type = VehicleType.Common, Year = 2020, Model = "Test Car"
        });
        _context.Auctions.Add(new Api.Shared.Entity.Auction
        {
            Id = _auctionId,
            SellerId = _sellerId,
            VehicleId = _vehicleId,
            BasePrice = 5000m,
            Price = 5000m,
            Status = AuctionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidBid_UpdatesAuctionPrice()
    {
        // Arrange
        var command = new PlaceBidCommand(_auctionId, _buyerId, 6000m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Price.Should().Be(6000m);
        result.BuyerId.Should().Be(_buyerId);

        var updatedAuction = await _context.Auctions.FindAsync(_auctionId);
        updatedAuction!.Price.Should().Be(6000m);
        updatedAuction.BuyerId.Should().Be(_buyerId);
    }

    [Fact]
    public async Task Handle_BidLowerThanCurrentPrice_ThrowsDomainException()
    {
        // Arrange
        var command = new PlaceBidCommand(_auctionId, _buyerId, 4000m); // lower than 5000

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*must be greater than*");
    }

    [Fact]
    public async Task Handle_BidEqualToCurrentPrice_ThrowsDomainException()
    {
        // Arrange
        var command = new PlaceBidCommand(_auctionId, _buyerId, 5000m); // equal

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_InactiveAuction_ThrowsDomainException()
    {
        // Arrange
        var auction = await _context.Auctions.FindAsync(_auctionId);
        auction!.Status = AuctionStatus.Inactive;
        await _context.SaveChangesAsync();

        var command = new PlaceBidCommand(_auctionId, _buyerId, 6000m);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task Handle_AuctionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new PlaceBidCommand(Guid.NewGuid(), _buyerId, 6000m);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ExpiredAuction_ThrowsDomainException()
    {
        // Arrange
        var auction = await _context.Auctions.FindAsync(_auctionId);
        auction!.EndDate = DateTime.UtcNow.AddDays(-1); // expired
        await _context.SaveChangesAsync();

        var command = new PlaceBidCommand(_auctionId, _buyerId, 6000m);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*ended*");
    }

    public void Dispose() => _context.Dispose();
}
