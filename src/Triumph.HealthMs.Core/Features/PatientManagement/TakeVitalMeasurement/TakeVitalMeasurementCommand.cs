namespace Triumph.HealthMs.Core.Features.PatientManagement.TakeVitalMeasurement;

public abstract record TakeVitalMeasurementCommand(IEnumerable<VitalMeasurementDto> VitalMeasurements)
{
    public Guid PatientId { get; set; }
    public Guid VisitationId { get; set; }
}

public abstract record VitalMeasurementDto(string VitalItemId, string MeasurementValue, string? Notes);

public class TakeVitalMeasurementCommandValidator : AbstractValidator<TakeVitalMeasurementCommand>
{
    public TakeVitalMeasurementCommandValidator()
    {
        RuleFor(x => x.VitalMeasurements)
            .NotNull().WithMessage("Vital measurements are required")
            .NotEmpty().WithMessage("Vital measurements cannot be empty");

        RuleForEach(x => new VitalMeasurementDtoValidator());
    }
}

public class VitalMeasurementDtoValidator : AbstractValidator<VitalMeasurementDto>
{
    public VitalMeasurementDtoValidator()
    {
        RuleFor(x => x.MeasurementValue)
            .NotEmpty().WithMessage("Measurement value is required")
            .MaximumLength(10).WithMessage("Measurement value should not be more than 10 characters");

        RuleFor(x => x.VitalItemId)
            .NotEmpty().WithMessage("Vital item is required")
            .NotNull();
        
        RuleFor(x => x.Notes)
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Notes contains invalid characters")
            .MaximumLength(500).WithMessage("Notes should not be more than 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}