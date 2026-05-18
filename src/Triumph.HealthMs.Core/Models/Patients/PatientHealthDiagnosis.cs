namespace Triumph.HealthMs.Core.Models.Patients;

public class PatientHealthDiagnosis : FacilityEntity
{
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public Guid? AssociatedVisit { get; set; }

    public Guid HealthDiagnosisId { get; set; }
    public HealthDiagnosis? HealthDiagnosis { get; set; }

    public string? ExtraNotes { get; set; }
    public bool ActivelyTreated { get; set; }
    public string? DiagnosedByFullname { get; set; }
    public DateOnly FirstDiagnosedAt { get; set; }
    public DateOnly? WasDeclaredRecoveredAt { get; set; }
    public string? HealthFacilityDiagnosedAt { get; set; }
}