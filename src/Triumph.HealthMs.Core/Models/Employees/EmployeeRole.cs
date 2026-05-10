namespace Triumph.HealthMs.Core.Models.Employees;

public class EmployeeRole : FacilityEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public DateTime StartedFrom { get; set; }
    public DateTime? EndedAt { get; set; }
}