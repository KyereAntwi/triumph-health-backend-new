namespace Triumph.HealthMs.Core.Features.ApplicationUser.UpdateUserInfomation;

public record UpdateUserInformationCommand(
    string FirstName,
    string LastName,
    string? OtherNames,
    string? Email,
    string PhoneNumber,
    string Gender,
    string Nationality,
    string DateOfBirth,
    string Title);

public class UpdateUserInformationCommandValidator : AbstractValidator<UpdateUserInformationCommand>
{
    public UpdateUserInformationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Must(x => Enum.TryParse<Title>(x, out _))
            .WithMessage("Title is invalid");
        
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
            .WithMessage("Date of birth is invalid");
        
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
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}