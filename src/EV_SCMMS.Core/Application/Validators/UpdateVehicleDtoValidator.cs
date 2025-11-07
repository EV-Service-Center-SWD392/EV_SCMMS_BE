using FluentValidation;

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
  public UpdateVehicleDtoValidator()
  {
    RuleFor(x => x.ModelId)
    .GreaterThan(0).WithMessage("ModelId must be greater than 0");

    RuleFor(x => x.LicensePlate)
        .MaximumLength(20)
        .When(x => !string.IsNullOrEmpty(x.LicensePlate))
        .WithMessage("LicensePlate must be at most 20 characters long");

    RuleFor(x => x.Year)
        .InclusiveBetween(1900, DateTime.Now.Year + 1)
        .When(x => x.Year != null)
        .WithMessage("Year must be between 1900 and the current year");

    RuleFor(x => x.Color)
        .MaximumLength(50)
        .When(x => !string.IsNullOrEmpty(x.Color))
        .WithMessage("Color must be at most 50 characters long");

    RuleFor(x => x.Status)
        .Must(BeValidStatus)
        .WithMessage("Status must be either ACTIVE or INACTIVE");

  }

  private bool BeValidStatus(string? status)
  {
    return status == null || new[] { "ACTIVE", "INACTIVE" }.Contains(status.ToUpper());
  }
}
