using FluentValidation;

public class BookingQueryDtoValidator : AbstractValidator<BookingQueryDto>
{
  public BookingQueryDtoValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page phải lớn hơn 0");
    RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize từ 1 đến 100");
    RuleFor(x => x.Status).Must(BeValidStatus).WithMessage("Status phải là một trong: " + string.Join(", ", BookingStatusConstant.BookingStatus))
        .When(x => !string.IsNullOrEmpty(x.Status));
    RuleFor(x => x.DayOfWeek).Must(BeValidDayOfWeek).WithMessage("DayOfWeek phải là một trong: " + string.Join(", ", DayOfWeekConstant.All))
        .When(x => !string.IsNullOrEmpty(x.DayOfWeek));
    RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate).WithMessage("FromDate không được sau ToDate")
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
