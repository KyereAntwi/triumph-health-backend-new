namespace Triumph.HealthMs.Core.Models.Employees;

public class EmployeeShift : FacilityEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; } = null!;
    public int ShiftDurationInHours { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime TimeStamp { get; set; }
    public ShiftType ShiftType { get; set; } = ShiftType.Morning;
    public DateTime? StartedAt { get; set; } = null;
    public DateTime? EndedAt { get; set; } = null;
}