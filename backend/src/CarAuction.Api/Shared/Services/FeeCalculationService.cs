namespace CarAuction.Api.Shared.Services;

public static class FeeCalculationService
{
    public static FeeCalculationResult Calculate(
        decimal basePrice,
        VehicleType vehicleType,
        BuyerFee buyerFeeConfig,
        SellerFee sellerFeeConfig,
        AssociationFee associationFeeConfig,
        StorageFee storageFeeConfig)
    {
        var basicFee = CalculateBuyerFee(basePrice, vehicleType, buyerFeeConfig);
        var specialFee = CalculateSellerFee(basePrice, vehicleType, sellerFeeConfig);
        var assocFee = associationFeeConfig.Fee;
        var storageFee = storageFeeConfig.Fee;
        var total = Math.Round(basePrice + basicFee + specialFee + assocFee + storageFee, 2);

        return new FeeCalculationResult
        {
            BasePrice = basePrice,
            VehicleType = vehicleType,
            BasicBuyerFee = basicFee,
            SpecialSellerFee = specialFee,
            AssociationFee = assocFee,
            StorageFee = storageFee,
            Total = total
        };
    }

    private static decimal CalculateBuyerFee(decimal price, VehicleType type, BuyerFee config)
    {
        var fee = Math.Round(price * config.FeePercentage, 2);
        return type == VehicleType.Common
            ? Math.Clamp(fee, config.FeeCommonMin, config.FeeCommonMax)
            : Math.Clamp(fee, config.FeeLuxuryMin, config.FeeLuxuryMax);
    }

    private static decimal CalculateSellerFee(decimal price, VehicleType type, SellerFee config)
    {
        var percentage = type == VehicleType.Common ? config.FeeCommon : config.FeeLuxury;
        return Math.Round(price * percentage, 2);
    }
}
