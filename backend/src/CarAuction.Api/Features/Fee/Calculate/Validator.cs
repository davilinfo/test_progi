using FluentValidation;

namespace CarAuction.Api.Features.Fee.Calculate;

public class CalculateFeeQueryValidator : AbstractValidator<CalculateFeeQuery>
{
    public CalculateFeeQueryValidator()
    {
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be greater than zero.");
        RuleFor(x => x.VehicleType).IsInEnum().WithMessage("Vehicle type must be Common or Luxury.");
    }
}
