namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddAUiStorageItem;

public record AddAUiStorageItemCommand(string Key, string Value);

public class AddAUiStorageItemCommandValidator : AbstractValidator<AddAUiStorageItemCommand>
{
    public AddAUiStorageItemCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key is required.")
            .MaximumLength(200)
            .WithMessage("Key must be less than 200 characters.");
        
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Value is required.")
            .MaximumLength(2000)
            .WithMessage("Value must be less than 2000 characters.");
    }
}