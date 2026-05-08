namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddManager;

public abstract record AddFacilityManagerCommand(
    string ApplicationUserId)
{
    public Guid FacilityId { get; set; }
}

public class AddFacilityManagerCommandValidator : AbstractValidator<AddFacilityManagerCommand>
{
    public AddFacilityManagerCommandValidator()
    {
        RuleFor(x => x.ApplicationUserId)
            .NotEmpty().WithMessage("Application user id is required")
            .NotNull();
    }
}