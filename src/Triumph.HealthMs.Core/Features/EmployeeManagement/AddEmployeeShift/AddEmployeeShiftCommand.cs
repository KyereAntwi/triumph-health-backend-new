namespace Triumph.HealthMs.Core.Features.EmployeeManagement.AddEmployeeShift;

public record AddEmployeeShiftCommand(
    int ShiftDurationInHours,
    string ShiftType,
    int DayOfWeek,
    string ShiftStartsAt,
    bool Recurring,
    string RecurringUntil,
    bool ArchivePreviouslyActiveOnes = true)
{
    public Guid EmployeeId { get; set; }
}

public class AddEmployeeShiftCommandValidator : AbstractValidator<AddEmployeeShiftCommand>
{
    public AddEmployeeShiftCommandValidator()
    {
        RuleFor(x => x.ShiftType)
            .NotEmpty()
            .WithMessage("Shift type is required.")
            .Must(x => Enum.TryParse<ShiftType>(x, out _))
            .WithMessage("Invalid shift type. Allowed values are: Morning, Afternoon, Evening, Night.");
        
        RuleFor(x => x.ShiftDurationInHours)
            .GreaterThan(0)
            .WithMessage("Shift duration must be greater than 0.")
            .LessThan(24)
            .WithMessage("Shift duration must be less than 24.");
        
        RuleFor(x => x.DayOfWeek)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Day of week must be greater than or equal to 0.")
            .LessThanOrEqualTo(6)
            .WithMessage("Day of week must be less than or equal to 6.");
        
        RuleFor(x => x.ShiftStartsAt)
            .NotEmpty()
            .WithMessage("Shift starts at is required")
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Invalid date format. Use 'yyyy-MM-dd'.");
        
        RuleFor(x => x.RecurringUntil)
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Invalid date format. Use 'yyyy-MM-dd'.")
            .When(x => x.Recurring);
    }
}