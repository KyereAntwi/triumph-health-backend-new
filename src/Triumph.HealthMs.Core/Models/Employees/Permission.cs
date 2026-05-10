namespace Triumph.HealthMs.Core.Models.Employees;

public class Permission : AuditableEntity
{
    public PermissionType PermissionType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}