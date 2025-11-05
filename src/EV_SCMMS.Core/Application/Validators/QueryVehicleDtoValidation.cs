using FluentValidation;

public class VehicleQueryDtoValidator : AbstractValidator<VehicleQueryDto>
{
  public VehicleQueryDtoValidator()
  {
    RuleFor(x => x.Page)
    .GreaterThan(0).WithMessage("Page must be greater than 0");

    RuleFor(x => x.PageSize)
        .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

    RuleFor(x => x.Status)
        .Must(BeValidStatus)
        .WithMessage("Status must be either ACTIVE or INACTIVE")
        .When(x => !string.IsNullOrEmpty(x.Status));

    RuleFor(x => x.Year)
        .InclusiveBetween(1900, DateTime.Now.Year + 1)
        .WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 1}")
        .When(x => x.Year.HasValue);

    RuleFor(x => x.FromDate)
        .LessThanOrEqualTo(x => x.ToDate)
        .WithMessage("FromDate cannot be later than ToDate")
        .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

  }

  private bool BeValidStatus(string? status)
  {
    return status?.ToUpper() switch
    {
      "ACTIVE" or "INACTIVE" => true,
      _ => false
    };
  }
}
