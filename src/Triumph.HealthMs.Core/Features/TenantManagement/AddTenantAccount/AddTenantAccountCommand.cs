namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;

public record AddTenantAccountCommand(
    string SubscriptionId,
    string SubscriptionChargeRate,
    string OrganizationalTitle,
    string Email,
    string Address,
    string MainTelephone);

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

        RuleFor(x => x.OrganizationalTitle)
            .NotEmpty().WithMessage("Organization title is required")
            .NotNull()
            .MaximumLength(100).WithMessage("Title should not be more than 100 characters")
            .MinimumLength(6).WithMessage("Title should not be less than 6 characters")
            .Matches(@"^[a-zA-Z0-9\s.\-&']+$").WithMessage("Organization title contains invalid characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not valid")
            .NotEmpty().WithMessage("Email should not be empty")
            .NotNull();

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address should not be more than 500 characters")
            .NotNull()
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Address contains invalid characters");

        RuleFor(x => x.MainTelephone)
            .NotEmpty().WithMessage("Telephone is required")
            .MaximumLength(15).WithMessage("Telephone should not be more than 15 characters")
            .MinimumLength(10).WithMessage("Telephone should not be less than 10 characters")
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Telephone contains invalid characters");
    }
}