using System.Globalization;
using FluentValidation;

public class CenterSchedulesQueryDtoValidator : AbstractValidator<CenterSchedulesQueryDto>
{
  public CenterSchedulesQueryDtoValidator()
  {
    RuleFor(x => x.StartDate)
    .NotEmpty().WithMessage("StartDate is required.")
    .Must(TimeHelper.BeValidDateFormat).When(x => !string.IsNullOrEmpty(x.StartDate)).WithMessage("StartDate must be in the format 'yyyy-MM-dd', e.g., '2025-11-03'.");

    RuleFor(x => x.EndDate)
        .NotEmpty().WithMessage("EndDate is required.")
        .Must(TimeHelper.BeValidDateFormat).When(x => !string.IsNullOrEmpty(x.EndDate)).WithMessage("EndDate must be in the format 'yyyy-MM-dd', e.g., '2025-11-09'.")
        .Must(BeEndDateAfterStartDate).When(x => !string.IsNullOrEmpty(x.EndDate) && !string.IsNullOrEmpty(x.StartDate)).WithMessage("EndDate must be the same as or after StartDate.");
  }


  private bool BeEndDateAfterStartDate(CenterSchedulesQueryDto dto, string endDate)
  {
    if (!DateOnly.TryParse(dto.StartDate, CultureInfo.InvariantCulture, out var parsedStart) ||
        !DateOnly.TryParse(endDate, CultureInfo.InvariantCulture, out var parsedEnd))
      return false;
    return parsedEnd >= parsedStart;
  }
}
