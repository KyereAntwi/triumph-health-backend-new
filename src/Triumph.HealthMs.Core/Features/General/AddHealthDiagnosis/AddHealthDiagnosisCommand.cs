namespace Triumph.HealthMs.Core.Features.General.AddHealthDiagnosis;

public record AddHealthDiagnosisCommand(
    string Name,
    string Description,
    string? RecommendedPrescription);

public class AddHealthDiagnosisCommandValidator : AbstractValidator<AddHealthDiagnosisCommand>
{
    public AddHealthDiagnosisCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(150)
            .WithMessage("Name cannot exceed 150 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");
        
        RuleFor(x => x.RecommendedPrescription)
            .MaximumLength(1000)
            .WithMessage("Prescription cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.RecommendedPrescription));
    }
}