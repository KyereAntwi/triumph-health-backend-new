namespace Triumph.HealthMs.Core.Models.Employees;

public class Department : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}