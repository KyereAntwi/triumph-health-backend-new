namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;

public record AddTenantAccountCommand(
    string SubscriptionId,
    string SubscriptionChargeRate);

public record AddTenantAccountResponse(
    Guid TenantId,
    string UniqueIdentifier);

public class AddTenantAccountCommandValidator : AbstractValidator<AddTenantAccountCommand>
{
    public AddTenantAccountCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription is required")
            .NotNull();

        RuleFor(x => x.SubscriptionChargeRate)
            .NotEmpty().WithMessage("Subscription charge rate is required")
            .NotNull()
            .Must(x => Enum.TryParse<SubscriptionChargeRate>(x, out _))
            .WithMessage("Subscription charge rate is invalid");
    }
}