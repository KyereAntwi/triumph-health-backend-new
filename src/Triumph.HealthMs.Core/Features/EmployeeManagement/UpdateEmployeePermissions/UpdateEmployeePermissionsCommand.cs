namespace Triumph.HealthMs.Core.Features.EmployeeManagement.UpdateEmployeePermissions;

public record UpdateEmployeePermissionsCommand(
    IEnumerable<string> Permissions)
{
    public Guid EmployeeId { get; set; }
}

public class UpdateEmployeePermissionsCommandValidator : AbstractValidator<UpdateEmployeePermissionsCommand>
{
    public UpdateEmployeePermissionsCommandValidator()
    {
        RuleFor(x => x.Permissions)
            .NotNull()
            .WithMessage("Permissions are required");
        
        RuleForEach(x => x.Permissions)
            .Must(x => Enum.TryParse<PermissionType>(x, out _))
            .WithMessage("Permission must be a valid permission");
    }
}