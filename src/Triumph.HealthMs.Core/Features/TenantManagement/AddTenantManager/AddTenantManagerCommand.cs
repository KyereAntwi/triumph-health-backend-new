namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantManager;

public record AddTenantManagerCommand(
    string ApplicationUserId);

public class AddTenantManagerCommandValidator : AbstractValidator<AddTenantManagerCommand>
{
    public AddTenantManagerCommandValidator()
    {
        RuleFor(x => x.ApplicationUserId)
            .NotEmpty().WithMessage("Application user id is required")
            .NotNull();
    }
}