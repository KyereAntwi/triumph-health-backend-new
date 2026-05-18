namespace Triumph.HealthMs.Core.Features.PatientManagement.UpdatePatient;

public abstract record UpdatePatientCommand(
    string FirstName,
    string LastName,
    string? Email,
    string PhoneNumber,
    string? OtherNames,
    string Gender,
    string Nationality,
    string DateOfBirth,
    string Address,
    string? PostGps)
{
    public Guid PatientId { get; set; }
}

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
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
        
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address should not be more than 500 characters")
            .NotNull()
            .Matches(@"^[a-zA-Z0-9\s,.\-/#()]+$").WithMessage("Address contains invalid characters");

        RuleFor(x => x.PostGps)
            .MaximumLength(15)
            .WithMessage("PostGps should not exceed 15 characters")
            .When(x => !string.IsNullOrEmpty(x.PostGps));
    }
}