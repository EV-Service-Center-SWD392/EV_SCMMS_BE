using FluentValidation;

public class VehicleQueryDtoValidator : AbstractValidator<VehicleQueryDto>
{
  public VehicleQueryDtoValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page phải lớn hơn 0");
    RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize từ 1 đến 100");
    RuleFor(x => x.Status).Must(BeValidStatus).WithMessage("Status phải là ACTIVE hoặc INACTIVE")
        .When(x => !string.IsNullOrEmpty(x.Status));
    RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.Now.Year + 1).WithMessage("Year hợp lệ từ 1900 đến năm hiện tại")
        .When(x => x.Year.HasValue);
    RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate).WithMessage("FromDate không được sau ToDate")
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
