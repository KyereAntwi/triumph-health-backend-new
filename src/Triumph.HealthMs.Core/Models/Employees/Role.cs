namespace Triumph.HealthMs.Core.Models.Employees;

public class Role : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}