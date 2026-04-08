using CarAuction.Api.Shared.Entity;
using CarAuction.Api.Shared.Enums;
using CarAuction.Api.Shared.Services;
using FluentAssertions;

namespace CarAuction.Tests.Features.Fee;

public class FeeCalculationServiceTests
{
    private static readonly BuyerFee DefaultBuyerFee = new()
    {
        Id = Guid.NewGuid(),
        FeePercentage = 0.10m,
        FeeCommonMin = 10m,
        FeeCommonMax = 50m,
        FeeLuxuryMin = 25m,
        FeeLuxuryMax = 200m
    };

    private static readonly SellerFee DefaultSellerFee = new()
    {
        Id = Guid.NewGuid(),
        FeeCommon = 0.02m,
        FeeLuxury = 0.04m
    };

    private static readonly StorageFee DefaultStorageFee = new()
    {
        Id = Guid.NewGuid(),
        Fee = 100m
    };

    private static AssociationFee GetAssocFee(decimal price) => price switch
    {
        <= 500m => new AssociationFee { Id = Guid.NewGuid(), Fee = 5m, MinRange = 1m, MaxRange = 500m },
        <= 1000m => new AssociationFee { Id = Guid.NewGuid(), Fee = 10m, MinRange = 500.01m, MaxRange = 1000m },
        <= 3000m => new AssociationFee { Id = Guid.NewGuid(), Fee = 15m, MinRange = 1000.01m, MaxRange = 3000m },
        _ => new AssociationFee { Id = Guid.NewGuid(), Fee = 20m, MinRange = 3000.01m, MaxRange = 9_999_999m }
    };

    // ── Test case 1: $398 Common ──────────────────────────────────────────────
    [Fact]
    public void Calculate_Common_398_ReturnsCorrectFees()
    {
        // Arrange
        const decimal basePrice = 398m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Common,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(39.80m);
        result.SpecialSellerFee.Should().Be(7.96m);
        result.AssociationFee.Should().Be(5.00m);
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(550.76m);
    }

    // ── Test case 2: $1000 Common ─────────────────────────────────────────────
    [Fact]
    public void Calculate_Common_1000_ReturnsCorrectFees()
    {
        // Arrange
        const decimal basePrice = 1000m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Common,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(50.00m);   // 10% = 100, capped at max $50
        result.SpecialSellerFee.Should().Be(20.00m); // 2%
        result.AssociationFee.Should().Be(10.00m);   // 500 < 1000 <= 1000
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(1180.00m);
    }

    // ── Test case 3: $1800 Luxury ─────────────────────────────────────────────
    [Fact]
    public void Calculate_Luxury_1800_ReturnsCorrectFees()
    {
        // Arrange
        const decimal basePrice = 1800m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Luxury,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(180.00m);  // 10% = 180, within [25,200]
        result.SpecialSellerFee.Should().Be(72.00m); // 4%
        result.AssociationFee.Should().Be(15.00m);   // 1000 < 1800 <= 3000
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(2167.00m);
    }

    // ── Test case 4: $50 Common — min buyer fee applies ───────────────────────
    [Fact]
    public void Calculate_Common_50_MinBuyerFeeApplies()
    {
        // Arrange
        const decimal basePrice = 50m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Common,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(10.00m);   // 10% of 50 = 5, but min is $10
        result.SpecialSellerFee.Should().Be(1.00m);  // 2% of 50
        result.AssociationFee.Should().Be(5.00m);
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(166.00m);
    }

    // ── Test case 5: $10 Common — min buyer fee applies ───────────────────────
    [Fact]
    public void Calculate_Common_10_MinBuyerFeeApplies()
    {
        // Arrange
        const decimal basePrice = 10m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Common,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(10.00m);   // 10% of 10 = 1, but min is $10
        result.SpecialSellerFee.Should().Be(0.20m);  // 2% of 10
        result.AssociationFee.Should().Be(5.00m);
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(125.20m);
    }

    // ── Test case 6: $3500 Luxury — max buyer fee + top association bracket ───
    [Fact]
    public void Calculate_Luxury_3500_MaxBuyerFeeAndTopAssociation()
    {
        // Arrange
        const decimal basePrice = 3500m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Luxury,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(200.00m);  // 10% = 350, capped at max $200
        result.SpecialSellerFee.Should().Be(140.00m); // 4%
        result.AssociationFee.Should().Be(20.00m);   // > 3000
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(3960.00m);
    }

    // ── Test case 7: $25 Luxury — min luxury buyer fee applies ───────────────
    [Fact]
    public void Calculate_Luxury_25_MinLuxuryBuyerFee()
    {
        // Arrange
        const decimal basePrice = 25m;

        // Act
        var result = FeeCalculationService.Calculate(
            basePrice, VehicleType.Luxury,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(basePrice), DefaultStorageFee);

        // Assert
        result.BasicBuyerFee.Should().Be(25.00m);  // 10% of 25 = 2.5, min is $25
        result.SpecialSellerFee.Should().Be(1.00m); // 4% of 25
        result.AssociationFee.Should().Be(5.00m);
        result.StorageFee.Should().Be(100.00m);
        result.Total.Should().Be(156.00m);
    }

    [Fact]
    public void Calculate_ReturnsCorrectVehicleTypeInResult()
    {
        var result = FeeCalculationService.Calculate(
            1000m, VehicleType.Luxury,
            DefaultBuyerFee, DefaultSellerFee,
            GetAssocFee(1000m), DefaultStorageFee);

        result.VehicleType.Should().Be(VehicleType.Luxury);
        result.BasePrice.Should().Be(1000m);
    }
}
