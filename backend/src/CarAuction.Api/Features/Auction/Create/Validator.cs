using FluentValidation;

namespace CarAuction.Api.Features.Auction.Create;

public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");
        RuleFor(x => x.EndDate).GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.");
    }
}
