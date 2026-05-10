namespace Triumph.HealthMs.Core.Models.Employees;

public class EmployeePermission : FacilityEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}