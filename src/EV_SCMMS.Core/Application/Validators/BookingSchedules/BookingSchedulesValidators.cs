using FluentValidation;

public class BookingSchedulesAttributesDtoValidator : AbstractValidator<BookingSchedulesAttributesDto>
{
    public BookingSchedulesAttributesDtoValidator()
    {
        RuleFor(x => x.Start)
            .NotEmpty()
            .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$")
            .WithMessage("Start time must be in format HH:mm");

        RuleFor(x => x.End)
            .NotEmpty()
            .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$")
            .WithMessage("End time must be in format HH:mm");

        RuleFor(x => x)
            .Must(x => BookingScheduleHelper.IsValidTimeRange(x.Start, x.End))
            .WithMessage("End time must be after start time");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .When(x => x.Capacity.HasValue);

        RuleFor(x => x.Note)
            .MaximumLength(250);

        RuleFor(x => x.Slot)
            .GreaterThan(0);

        RuleFor(x => x.DayOfWeek)
            .Must(d => DayOfWeekConstant.All.Contains(d))
            .WithMessage("Invalid day of week");
    }
}

public class BookingScheduleDtoValidator : AbstractValidator<BookingScheduleDto>
{
    public BookingScheduleDtoValidator()
    {
        RuleFor(x => x.CenterId).NotEmpty();

        RuleForEach(x => x.CenterSchedules)
            .SetValidator(new BookingSchedulesAttributesDtoValidator());

        RuleFor(x => x.CenterSchedules).Must(BookingScheduleHelper.IsValidTime).WithMessage("Time was not valid");

        RuleFor(x => x.CenterSchedules)
            .Must(BookingScheduleHelper.NoOverlap)
            .WithMessage("Slots must not overlap for the same day");
    }
}
