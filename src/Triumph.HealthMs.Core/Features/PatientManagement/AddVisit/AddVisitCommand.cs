namespace Triumph.HealthMs.Core.Features.PatientManagement.AddVisit;

public abstract record AddVisitCommand(string VisitReason)
{
    public Guid PatientId { get; set; }
}

public class AddVisitCommandValidator : AbstractValidator<AddVisitCommand>
{
    public AddVisitCommandValidator()
    {
        RuleFor(x => x.VisitReason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Visitation reason should not be more than 500 characters")
            .NotNull()
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Reason contains invalid characters");
    }
}