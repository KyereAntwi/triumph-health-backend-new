namespace Triumph.HealthMs.Core.Features.General.AddLabTest;

public record AddLabTestCommand(
    string Name,
    string Description);
    
public class AddLabTestCommandValidator : AbstractValidator<AddLabTestCommand>
{
    public AddLabTestCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");
            
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}