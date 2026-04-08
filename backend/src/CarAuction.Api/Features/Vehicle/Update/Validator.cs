using FluentValidation;

namespace CarAuction.Api.Features.Vehicle.Update;

public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Plate).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
    }
}
