using System.Globalization;
using FluentValidation;

public class CenterSchedulesQueryDtoValidator : AbstractValidator<CenterSchedulesQueryDto>
{
  public CenterSchedulesQueryDtoValidator()
  {
    RuleFor(x => x.StartDate)
        .NotEmpty().WithMessage("StartDate là bắt buộc")
        .Must(TimeHelper.BeValidDateFormat).WithMessage("StartDate phải có format 'yyyy-MM-dd', ví dụ: '2025-11-03'");

    RuleFor(x => x.EndDate)
        .NotEmpty().WithMessage("EndDate là bắt buộc")
        .Must(TimeHelper.BeValidDateFormat).WithMessage("EndDate phải có format 'yyyy-MM-dd', ví dụ: '2025-11-09'")
        .Must(BeEndDateAfterStartDate).WithMessage("EndDate phải sau hoặc bằng StartDate");
  }


  private bool BeEndDateAfterStartDate(CenterSchedulesQueryDto dto, string endDate)
  {
    if (!DateOnly.TryParse(dto.StartDate, CultureInfo.InvariantCulture, out var parsedStart) ||
        !DateOnly.TryParse(endDate, CultureInfo.InvariantCulture, out var parsedEnd))
      return false;
    return parsedEnd >= parsedStart;
  }
}
