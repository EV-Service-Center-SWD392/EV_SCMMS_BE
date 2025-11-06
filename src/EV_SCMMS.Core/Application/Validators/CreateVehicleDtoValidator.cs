using FluentValidation;

public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{

  public CreateVehicleDtoValidator()
  {
    RuleFor(x => x.ModelId).GreaterThan(0).WithMessage("ModelId phải > 0");
    RuleFor(x => x.LicensePlate).NotEmpty().MaximumLength(20).WithMessage("LicensePlate 1-20 ký tự");
    RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.Now.Year + 1).When(x => x.Year.HasValue).WithMessage("Year hợp lệ: 1900-nay");
    RuleFor(x => x.Color).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Color)).WithMessage("Color tối đa 50 ký tự");
    RuleFor(x => x.Status).Must(BeValidStatus).WithMessage("Status phải là ACTIVE hoặc INACTIVE");
    RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive bắt buộc");
  }

  private bool BeValidStatus(string? status)
  {
    return status == null || new[] { "ACTIVE", "INACTIVE" }.Contains(status.ToUpper());
  }
}
