using FluentValidation;

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
  public UpdateVehicleDtoValidator()
  {
    RuleFor(x => x.ModelId).GreaterThan(0).WithMessage("ModelId phải > 0");
    RuleFor(x => x.LicensePlate).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.LicensePlate)).WithMessage("LicensePlate tối đa 20 ký tự");
    RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.Now.Year + 1).When(x => x.Year != null).WithMessage("Year hợp lệ");
    RuleFor(x => x.Color).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Color)).WithMessage("Color tối đa 50 ký tự");
    RuleFor(x => x.Status).Must(BeValidStatus).WithMessage("Status phải là ACTIVE hoặc INACTIVE");
  }

  private bool BeValidStatus(string? status)
  {
    return status == null || new[] { "ACTIVE", "INACTIVE" }.Contains(status.ToUpper());
  }
}
