namespace Triumph.HealthMs.Core.Models.Facilities;

public class FacilityManager : FacilityEntity
{
    public OrganizationalFacility? OrganizationalFacility { get; set; }

    public Guid ApplicationUserId { get; set; }
}