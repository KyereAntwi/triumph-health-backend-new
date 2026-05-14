namespace Triumph.HealthMs.Core.Models.Patients;

public class PatientVital : FacilityEntity
{
    public Guid VitalItemId { get; set; }
    public VitalItem? VitalItem { get; set; }

    public Guid VisitationId { get; set; }
    public Visitation? Visitation { get; set; }

    public string MeasurementValue { get; set; } = string.Empty;
    public string? Notes { get; set; }
}