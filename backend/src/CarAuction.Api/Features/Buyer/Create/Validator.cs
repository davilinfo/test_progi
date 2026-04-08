using FluentValidation;

namespace CarAuction.Api.Features.Buyer.Create;

public class CreateBuyerCommandValidator : AbstractValidator<CreateBuyerCommand>
{
    public CreateBuyerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Age).InclusiveBetween(18, 120);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
