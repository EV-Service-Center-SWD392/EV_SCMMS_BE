using FluentValidation;

public class BookingQueryDtoValidator : AbstractValidator<BookingQueryDto>
{
  public BookingQueryDtoValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be larger than 0");
    RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be from 1 to 100");
    RuleFor(x => x.Status).Must(BeValidStatus).WithMessage("Status must be one of: " + string.Join(", ", BookingStatusConstant.BookingStatus))
        .When(x => !string.IsNullOrEmpty(x.Status));
    RuleFor(x => x.DayOfWeek).Must(BeValidDayOfWeek).WithMessage("DayOfWeek must be one of: " + string.Join(", ", DayOfWeekConstant.All))
        .When(x => !string.IsNullOrEmpty(x.DayOfWeek));
    RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate).WithMessage("From Date must not been behind To Date")
        .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
  }

  private bool BeValidStatus(string? status)
  {
    if (string.IsNullOrWhiteSpace(status)) return true;
    return BookingStatusConstant.BookingStatus.Contains(status, StringComparer.OrdinalIgnoreCase);
  }

  private bool BeValidDayOfWeek(string? dayOfWeek)
  {
    if (string.IsNullOrWhiteSpace(dayOfWeek)) return true;
    return DayOfWeekConstant.All.Contains(dayOfWeek, StringComparer.OrdinalIgnoreCase);
  }
}
