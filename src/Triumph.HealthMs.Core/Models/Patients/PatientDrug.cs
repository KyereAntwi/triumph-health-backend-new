namespace Triumph.HealthMs.Core.Models.Patients;

public class PatientDrug : FacilityEntity
{
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public Guid DrugId { get; set; }
    public string? ExtraNotes { get; set; }
    public Guid? AssociatedVisit { get; set; }
    public Guid? AssociatedDiagnosis { get; set; }
    public bool ActivelyTaking { get; set; }
    public int QuantityTaking { get; set; }
    public decimal AmountPaidPerDrug { get; set; }
}