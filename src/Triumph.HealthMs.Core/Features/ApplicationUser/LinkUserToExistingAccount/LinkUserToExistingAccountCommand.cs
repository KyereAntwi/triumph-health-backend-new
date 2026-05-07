namespace Triumph.HealthMs.Core.Features.ApplicationUser.LinkUserToExistingAccount;

public record LinkUserToExistingAccountCommand(string LinkId);

public class LinkUserToExistingAccountCommandValidator : AbstractValidator<LinkUserToExistingAccountCommand>
{
    public LinkUserToExistingAccountCommandValidator()
    {
        RuleFor(x => x.LinkId)
            .NotEmpty()
            .WithMessage("LinkId is required.");
    }
}