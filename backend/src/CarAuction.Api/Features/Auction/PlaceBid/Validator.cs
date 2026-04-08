using FluentValidation;

namespace CarAuction.Api.Features.Auction.PlaceBid;

public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.BidAmount).GreaterThan(0).WithMessage("Bid amount must be greater than zero.");
    }
}
