namespace Triumph.HealthMs.Core.Models.Patients;

public class Visitation : FacilityEntity
{
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public string VisitingReason { get; set; } = string.Empty;

    public ICollection<PatientVital> PatientVitals { get; set; } = [];
    public ICollection<Consultation> Consultations { get; set; } = [];
    public ICollection<PatientLabTest> PatientLabTests { get; set; } = [];
}