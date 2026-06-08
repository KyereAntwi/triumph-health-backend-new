namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddLinkInvitation;

public record AddLinkInvitationCommand(
    string EntityType,
    string EntityId);

public class AddLinkInvitationCommandValidator : AbstractValidator<AddLinkInvitationCommand>
{
    public AddLinkInvitationCommandValidator()
    {
        RuleFor(x => x.EntityType)
            .NotEmpty()
            .WithMessage("Entity type is required.")
            .Must(x => x is "Patient" or "Employee");

        RuleFor(x => x.EntityId)
            .NotEmpty()
            .WithMessage("Entity ID is required.");
    }
}