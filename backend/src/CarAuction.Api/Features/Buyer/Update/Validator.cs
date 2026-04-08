using FluentValidation;

namespace CarAuction.Api.Features.Buyer.Update;

public class UpdateBuyerCommandValidator : AbstractValidator<UpdateBuyerCommand>
{
    public UpdateBuyerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Age).InclusiveBetween(18, 120);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
