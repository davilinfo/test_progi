using FluentValidation;

namespace CarAuction.Api.Features.Seller.Update;

public class UpdateSellerCommandValidator : AbstractValidator<UpdateSellerCommand>
{
    public UpdateSellerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
