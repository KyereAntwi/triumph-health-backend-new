namespace Triumph.HealthMs.Core.Models.Employees;

public class Employee : FacilityEntity
{
    public string UniqueIdentifier { get; set; } = string.Empty;
    public Guid ApplicationUserId { get; set; }
    public DateOnly? EmployedAt { get; set; }

    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    

    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = [];
    public ICollection<EmployeePermission> EmployeePermissions { get; set; } = [];
    public ICollection<EmploymentAttachment> EmploymentAttachments { get; set; } = [];
    public ICollection<EmployeeActivity> EmployeeActivities { get; set; } = [];
}