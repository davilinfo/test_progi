using CarAuction.Api.Features.Auction.Close;
using CarAuction.Api.Features.Auction.Repository;
using CarAuction.Api.Features.Fee.Repository;
using CarAuction.Api.Shared.Entity;
using CarAuction.Api.Shared.Enums;
using CarAuction.Api.Shared.Exceptions;
using CarAuction.Api.Shared.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Tests.Features.Auction;

public class CloseAuctionHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IAuctionRepository> _auctionRepoMock;
    private readonly Mock<IFeeRepository> _feeRepoMock;
    private readonly Mock<ILogger<CloseAuctionHandler>> _loggerMock;
    private readonly CloseAuctionHandler _handler;

    private readonly Guid _auctionId = Guid.NewGuid();
    private readonly Guid _sellerId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();

    public CloseAuctionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"CloseAuctionTest_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);

        _auctionRepoMock = new Mock<IAuctionRepository>();
        _feeRepoMock = new Mock<IFeeRepository>();
        _loggerMock = new Mock<ILogger<CloseAuctionHandler>>();

        _handler = new CloseAuctionHandler(
            _auctionRepoMock.Object,
            _feeRepoMock.Object,
            _context,
            _loggerMock.Object);

        SeedTestData();
        SetupFeeRepoMocks();
    }

    private void SeedTestData()
    {
        _context.Sellers.Add(new Seller { Id = _sellerId, Name = "Test Seller", Phone = "555-0001", Email = "seller@test.com" });
        _context.Vehicles.Add(new Api.Shared.Entity.Vehicle
        {
            Id = _vehicleId, Plate = "TST-001", Type = VehicleType.Common, Year = 2020, Model = "Test Car"
        });
        _context.Auctions.Add(new Api.Shared.Entity.Auction
        {
            Id = _auctionId,
            SellerId = _sellerId,
            VehicleId = _vehicleId,
            BasePrice = 1000m,
            Price = 1000m,
            Status = AuctionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(-1)
        });
        _context.SaveChanges();
    }

    private void SetupFeeRepoMocks()
    {
        _feeRepoMock.Setup(f => f.GetBuyerFeeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BuyerFee
            {
                Id = Guid.NewGuid(),
                FeePercentage = 0.10m,
                FeeCommonMin = 10m, FeeCommonMax = 50m,
                FeeLuxuryMin = 25m, FeeLuxuryMax = 200m
            });

        _feeRepoMock.Setup(f => f.GetSellerFeeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SellerFee { Id = Guid.NewGuid(), FeeCommon = 0.02m, FeeLuxury = 0.04m });

        _feeRepoMock.Setup(f => f.GetAssociationFeeByPriceAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssociationFee { Id = Guid.NewGuid(), Fee = 10m, MinRange = 500.01m, MaxRange = 1000m });

        _feeRepoMock.Setup(f => f.GetStorageFeeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StorageFee { Id = Guid.NewGuid(), Fee = 100m });
    }

    [Fact]
    public async Task Handle_ActiveAuction_ClosesAndReturnsFees()
    {
        // Arrange
        var command = new CloseAuctionCommand(_auctionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Auction.Status.Should().Be(AuctionStatus.Inactive);
        result.Fees.Should().NotBeNull();
        result.Fees.Total.Should().Be(1180.00m); // 1000 + 50 + 20 + 10 + 100

        var closedAuction = await _context.Auctions.FindAsync(_auctionId);
        closedAuction!.Status.Should().Be(AuctionStatus.Inactive);
    }

    [Fact]
    public async Task Handle_AlreadyInactiveAuction_ThrowsDomainException()
    {
        // Arrange
        var auction = await _context.Auctions.FindAsync(_auctionId);
        auction!.Status = AuctionStatus.Inactive;
        await _context.SaveChangesAsync();

        var command = new CloseAuctionCommand(_auctionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*already closed*");
    }

    [Fact]
    public async Task Handle_NonExistentAuction_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CloseAuctionCommand(Guid.NewGuid());

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsCorrectFeeBreakdown()
    {
        // Arrange
        var command = new CloseAuctionCommand(_auctionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Fees.BasePrice.Should().Be(1000m);
        result.Fees.BasicBuyerFee.Should().Be(50m);    // capped at $50
        result.Fees.SpecialSellerFee.Should().Be(20m); // 2% of 1000
        result.Fees.AssociationFee.Should().Be(10m);   // mocked
        result.Fees.StorageFee.Should().Be(100m);
        result.Fees.Total.Should().Be(1180m);
    }

    public void Dispose() => _context.Dispose();
}
