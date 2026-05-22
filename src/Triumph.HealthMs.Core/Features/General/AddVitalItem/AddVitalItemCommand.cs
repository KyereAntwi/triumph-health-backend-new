namespace Triumph.HealthMs.Core.Features.General.AddVitalItem;

public record AddVitalItemCommand(
    string Name,
    string Description);

public class AddVitalItemCommandValidator : AbstractValidator<AddVitalItemCommand>
{
    public AddVitalItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name field should not be more than 100 characters");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description field should not be more than 500 characters");
    }
}