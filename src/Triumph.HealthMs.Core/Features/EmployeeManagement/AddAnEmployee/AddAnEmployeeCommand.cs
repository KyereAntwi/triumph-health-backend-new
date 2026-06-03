namespace Triumph.HealthMs.Core.Features.EmployeeManagement.AddAnEmployee;

public record AddAnEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? OtherNames,
    string Gender,
    string Nationality,
    string DateOfBirth,
    string FacilityId,
    string RoleId,
    string StartedRoleFrom,
    string? EndedRoleAt,
    string EmployedAt,
    IEnumerable<string>? Permissions,
    bool SetAsFacilityManager,
    string DepartmentId);

public class AddAnEmployeeCommandValidator : AbstractValidator<AddAnEmployeeCommand>
{
    public AddAnEmployeeCommandValidator()
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
            .WithMessage("Email is invalid");

        RuleFor(x => x.EmployedAt)
            .NotEmpty()
            .WithMessage("Employed at is required")
            .Must(x => DateTime.Parse(x) <= DateTime.UtcNow)
            .WithMessage("Employed at must be a valid date and should not be more than current date");
        
        RuleFor(x => x.StartedRoleFrom)
            .NotEmpty()
            .WithMessage("Date role was started is required")
            .Must(x => DateTime.Parse(x) <= DateTime.UtcNow)
            .WithMessage("Role started from must be a valid date and should not be more than current date");
        
        RuleFor(x => x.EndedRoleAt)
            .Must(x => DateTime.Parse(x!) <= DateTime.UtcNow)
            .WithMessage("Role ended from must be a valid date and should not be more than current date")
            .When(x => !string.IsNullOrEmpty(x.EndedRoleAt));

        RuleForEach(x => x.Permissions)
            .Must(x => Enum.TryParse<PermissionType>(x, out _))
            .WithMessage("Permission must be a valid permission")
            .When(x => x.Permissions != null && x.Permissions.Any());

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role Id is required")
            .NotNull();

        RuleFor(x => x.FacilityId)
            .NotEmpty()
            .WithMessage("Facility Id is required")
            .NotNull();

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("Department id is required")
            .NotNull();
    }
}