namespace Triumph.HealthMs.Core.Models.Patients;

public class Identification : FacilityEntity
{
    public IdentificationType Type { get; set; } = IdentificationType.NationalIdCard;
    public string IdentificationNumber { get; set; } = string.Empty;
    public DateOnly DateIssued { get; set; }
    public DateOnly DateExpires { get; set; }
    public string PlaceOfIssue { get; set; } = string.Empty;
    public string CountryOfIssue { get; set; } = string.Empty;

    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }
}