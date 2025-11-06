using FluentValidation;

public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{

  public CreateVehicleDtoValidator()
  {
    RuleFor(x => x.ModelId)
    .GreaterThan(0).WithMessage("ModelId must be greater than 0");

    RuleFor(x => x.LicensePlate)
        .NotEmpty().MaximumLength(20).WithMessage("LicensePlate must be between 1 and 20 characters");

    RuleFor(x => x.Year)
        .InclusiveBetween(1900, DateTime.Now.Year + 1)
        .When(x => x.Year.HasValue)
        .WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 1}");

    RuleFor(x => x.Color)
        .MaximumLength(50)
        .When(x => !string.IsNullOrEmpty(x.Color))
        .WithMessage("Color can have a maximum of 50 characters");

    RuleFor(x => x.Status)
        .Must(BeValidStatus)
        .WithMessage("Status must be either ACTIVE or INACTIVE");

    RuleFor(x => x.IsActive)
        .NotNull().WithMessage("IsActive is required");

  }

  private bool BeValidStatus(string? status)
  {
    return status == null || new[] { "ACTIVE", "INACTIVE" }.Contains(status.ToUpper());
  }
}
