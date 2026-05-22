namespace Triumph.HealthMs.Core.Features.General.AddADrug;

public record AddADrugCommand(
    string Name,
    string Description,
    string Prescription,
    string? Manufacturer);

public class AddADrugCommandValidator : AbstractValidator<AddADrugCommand>
{
    public AddADrugCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Prescription)
            .NotEmpty()
            .WithMessage("Prescription is required.")
            .MaximumLength(1000)
            .WithMessage("Prescription must not exceed 1000 characters.");
        
        RuleFor(x => x.Manufacturer)
            .MaximumLength(100)
            .WithMessage("Manufacturer must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Manufacturer));
    }
}