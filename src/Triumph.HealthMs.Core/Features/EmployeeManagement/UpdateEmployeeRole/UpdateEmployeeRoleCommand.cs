namespace Triumph.HealthMs.Core.Features.EmployeeManagement.UpdateEmployeeRole;

public abstract record UpdateEmployeeRoleCommand(
    string RoleId,
    string StartsAt,
    string OldRoleEndedAt)
{
    public Guid EmployeeId { get; set; }
}

public class UpdateEmployeeRoleCommandValidator : AbstractValidator<UpdateEmployeeRoleCommand>
{
    public UpdateEmployeeRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required")
            .NotNull();
        
        RuleFor(x => x.StartsAt)
            .NotEmpty().WithMessage("Starting date is required")
            .Must(x => DateTime.Parse(x) <= DateTime.UtcNow)
            .WithMessage("Starting date should not be in the future")
            .NotNull();
        
        RuleFor(x => x.OldRoleEndedAt)
            .NotEmpty().WithMessage("Old role ended date date is required")
            .Must(x => DateTime.Parse(x) <= DateTime.UtcNow)
            .WithMessage("Old role ended date should not be in the future")
            .NotNull();
    }
}