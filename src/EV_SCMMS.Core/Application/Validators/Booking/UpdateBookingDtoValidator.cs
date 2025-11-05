using FluentValidation;

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
  public UpdateBookingDtoValidator()
  {
    Include(new CreateBookingDtoValidator());

    RuleFor(x => x.IsCancel).NotNull().WithMessage("IsCancel flag is required");
  }
}
