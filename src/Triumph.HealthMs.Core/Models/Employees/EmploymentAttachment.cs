namespace Triumph.HealthMs.Core.Models.Employees;

public class EmploymentAttachment : FacilityEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public string AttachmentUrl { get; set; } = string.Empty;
    public string AttachmentType { get; set; } = string.Empty;
}