namespace Triumph.HealthMs.Core.Models.Patients;

public class Patient : FacilityEntity
{
    public Guid ApplicationUserId { get; set; }
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PostGps { get; set; }

    public ICollection<Identification> Identifications { get; set; } = [];
    public ICollection<PatientHealthDiagnosis> PatientHealthDiagnoses { get; set; } = [];
    public ICollection<Visitation> Visitations { get; set; } = [];
    public ICollection<PatientDrug> PatientDrugs { get; set; } = [];
}