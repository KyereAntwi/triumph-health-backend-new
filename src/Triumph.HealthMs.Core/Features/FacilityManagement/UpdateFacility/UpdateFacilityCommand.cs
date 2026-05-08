namespace Triumph.HealthMs.Core.Features.FacilityManagement.UpdateFacility;

public abstract record UpdateFacilityCommand(
    string Name,
    string Address,
    string Email,
    string MainTelePhone,
    string? Description,
    string? EstablishedAt)
{
    public Guid? Id { get; set; }
}

public sealed class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name title is required")
            .NotNull()
            .MaximumLength(100).WithMessage("Name should not be more than 100 characters")
            .MinimumLength(6).WithMessage("Name should not be less than 6 characters")
            .Matches(@"^[a-zA-Z0-9\s.\-&']+$").WithMessage("Facility name contains invalid characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description should not be more than 500 characters")
            .Matches(@"^[a-zA-Z0-9\s.\-&']+$").WithMessage("Description contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EstablishedAt)
            .Must(x => x != null && DateTime.Parse(x, CultureInfo.InvariantCulture).Date <= DateTime.UtcNow.Date)
            .WithMessage("Established date should not be in the future")
            .When(x => !string.IsNullOrEmpty(x.EstablishedAt));
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not valid")
            .NotEmpty().WithMessage("Email should not be empty")
            .NotNull();

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address should not be more than 500 characters")
            .NotNull()
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Address contains invalid characters");

        RuleFor(x => x.MainTelePhone)
            .NotEmpty().WithMessage("Telephone is required")
            .MaximumLength(15).WithMessage("Telephone should not be more than 15 characters")
            .MinimumLength(10).WithMessage("Telephone should not be less than 10 characters")
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Telephone contains invalid characters");
    }
}