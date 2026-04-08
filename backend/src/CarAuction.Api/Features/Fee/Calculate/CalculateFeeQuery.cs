namespace CarAuction.Api.Features.Fee.Calculate;

public record CalculateFeeQuery(decimal BasePrice, VehicleType VehicleType) : IRequest<FeeCalculationResult>;
