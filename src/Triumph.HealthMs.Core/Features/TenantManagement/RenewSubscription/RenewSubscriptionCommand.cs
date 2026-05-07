namespace Triumph.HealthMs.Core.Features.TenantManagement.RenewSubscription;

public record RenewSubscriptionCommand(
    string SubscriptionId,
    string SubscriptionChargeRate);


public sealed class RenewSubscriptionCommandValidator : AbstractValidator<RenewSubscriptionCommand>
{
    public RenewSubscriptionCommandValidator()
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