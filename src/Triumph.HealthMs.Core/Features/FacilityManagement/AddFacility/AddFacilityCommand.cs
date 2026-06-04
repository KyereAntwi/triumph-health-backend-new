namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddFacility;

public record AddFacilityCommand(
    string UrlSuffix,
    string Name,
    string Address,
    string Email,
    string MainTelephone,
    string? Description,
    string? EstablishedAt);

public sealed class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
{
    public AddFacilityCommandValidator()
    {
        // no spaces allowed
        // only ('.' and '-') special characters are allowed
        RuleFor(x => x.UrlSuffix)
            .NotEmpty().WithMessage("UrlSuffix is required")
            .MaximumLength(30).WithMessage("UrlSuffix should not be more than 30 characters")
            .Matches(@"^[a-zA-Z0-9.\-]+$").WithMessage("UrlSuffix contains invalid characters");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name title is required")
            .NotNull()
            .MaximumLength(100).WithMessage("Name should not be more than 100 characters")
            .MinimumLength(6).WithMessage("Name should not be less than 6 characters");
        
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

        RuleFor(x => x.MainTelephone)
            .NotEmpty().WithMessage("Telephone is required")
            .MaximumLength(15).WithMessage("Telephone should not be more than 15 characters")
            .MinimumLength(10).WithMessage("Telephone should not be less than 10 characters")
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Telephone contains invalid characters");
    }
}