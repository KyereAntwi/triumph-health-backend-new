namespace Triumph.HealthMs.Core.Models.Facilities;

public class FacilityLabTest : FacilityEntity
{
    public OrganizationalFacility? Facility { get; set; }

    public string UniqueIdentifier { get; set; } = string.Empty;
    public Guid LabTestId { get; set; }
    public LabTest? LabTest { get; set; }
    
    public decimal CurrentValue { get; set; }
    public string? AdditionalFacilityNotes { get; set; }
}