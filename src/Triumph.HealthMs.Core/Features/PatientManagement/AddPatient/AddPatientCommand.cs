namespace Triumph.HealthMs.Core.Features.PatientManagement.AddPatient;

public abstract record AddPatientCommand(
    string FirstName,
    string LastName,
    string? Email,
    string PhoneNumber,
    string? OtherNames,
    string Gender,
    string Nationality,
    string DateOfBirth,
    string Address,
    string? PostGps,
    IEnumerable<IdentificationDto>? Identifications,
    bool SendAccountLinkageInvitation);
    
public abstract record IdentificationDto(
    string Type,
    string IdentificationNumber,
    string DateIssued,
    string DateExpires,
    string PlaceOfIssue,
    string CountryOfIssue);

public class AddPatientCommandValidator : AbstractValidator<AddPatientCommand>
{
    public AddPatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(15)
            .WithMessage("First name cannot exceed 15 characters");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(15)
            .WithMessage("Last name cannot exceed 15 characters");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(15)
            .WithMessage("Phone number cannot exceed 15 characters");
        
        RuleFor(x => x.Nationality)
            .NotEmpty()
            .WithMessage("Nationality is required")
            .Must(x => Enum.TryParse<Nationality>(x, out _))
            .WithMessage("Nationality is invalid");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Date of birth is invalid")
            .Must(x => DateTime.Parse(x).Date < DateTime.UtcNow.Date)
            .WithMessage("Date of birth should be less than today");
        
        RuleFor(x => x.OtherNames)
            .MaximumLength(15)
            .WithMessage("Other names cannot exceed 15 characters")
            .When(x => !string.IsNullOrEmpty(x.OtherNames));
        
        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required")
            .Must(x => Enum.TryParse<Gender>(x, out _))
            .WithMessage("Gender is invalid");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is invalid")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .When(x => x.SendAccountLinkageInvitation);
        
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address should not be more than 500 characters")
            .NotNull()
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Address contains invalid characters");

        RuleFor(x => x.PostGps)
            .MaximumLength(15)
            .WithMessage("PostGps should not exceed 15 characters")
            .When(x => !string.IsNullOrEmpty(x.PostGps));

        RuleForEach(x => x.Identifications)
            .SetValidator(new IdentificationDtoValidator())
            .When(x => (x.Identifications != null || x.Identifications!.Any()));
    }
}

public class IdentificationDtoValidator : AbstractValidator<IdentificationDto>
{
    public IdentificationDtoValidator()
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