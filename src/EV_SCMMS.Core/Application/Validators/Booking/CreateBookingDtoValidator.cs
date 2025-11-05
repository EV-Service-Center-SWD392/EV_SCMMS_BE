using FluentValidation;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
  public CreateBookingDtoValidator()
  {
    RuleFor(x => x.Slot).NotEmpty().GreaterThan(0).WithMessage("Slot is required");
    RuleFor(x => x.BookingDate)
                .NotEmpty().WithMessage("BookingDate is required")
                .Must(TimeHelper.BeValidDateFormat).WithMessage("BookingDate must be in the exact format 'yyyy-MM-dd', e.g. '2025-01-01' (no hour, no timezone)");

    RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle is required");
    RuleFor(x => x.CenterId).NotEmpty().WithMessage("Center is required");
    RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Notes));
  }
}
