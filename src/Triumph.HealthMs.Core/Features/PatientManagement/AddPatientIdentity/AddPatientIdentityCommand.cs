namespace Triumph.HealthMs.Core.Features.PatientManagement.AddPatientIdentity;

public abstract record AddPatientIdentityCommand(
    string Type,
    string IdentificationNumber,
    string DateIssued,
    string DateExpires,
    string PlaceOfIssue,
    string CountryOfIssue)
{
    public Guid PatientId { get; set; }
}

public class AddPatientIdentityCommandValidator : AbstractValidator<AddPatientIdentityCommand>
{
    public AddPatientIdentityCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Identification Type is required")
            .Must(x => Enum.TryParse<IdentificationType>(x, out _))
            .WithMessage("Identification is invalid");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithMessage("Identification number is required")
            .MaximumLength(15).WithMessage("Number should not be more than 15 characters")
            .MinimumLength(10).WithMessage("Number should not be less than 10 characters")
            .NotNull();
        
        RuleFor(x => x.DateIssued)
            .NotEmpty()
            .WithMessage("Date Issued is required")
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Date Issued is invalid")
            .Must(x => DateTime.Parse(x).Date <= DateTime.UtcNow.Date)
            .WithMessage("Date Issued should not be in the future");
        
        RuleFor(x => x.DateExpires)
            .NotEmpty()
            .WithMessage("Date Expires is required")
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Date Expires is invalid")
            .Must(x => DateTime.Parse(x).Date > DateTime.UtcNow.Date)
            .WithMessage("Date Expires is expired");
        
        RuleFor(x => x.PlaceOfIssue)
            .NotEmpty().WithMessage("Place of issue is required")
            .MaximumLength(100).WithMessage("Place of issue should not be more than 100 characters")
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Place of issue contains invalid characters")
            .NotNull();
        
        RuleFor(x => x.CountryOfIssue)
            .NotEmpty().WithMessage("Country of issue is required")
            .MaximumLength(100).WithMessage("Country of issue should not be more than 100 characters")
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Country of issue contains invalid characters")
            .NotNull();
    }
}