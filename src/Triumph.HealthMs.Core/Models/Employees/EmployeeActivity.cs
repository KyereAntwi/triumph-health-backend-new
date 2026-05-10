namespace Triumph.HealthMs.Core.Models.Employees;

public class EmployeeActivity : FacilityEntity
{
    public string Action { get; set; } = string.Empty;

    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}