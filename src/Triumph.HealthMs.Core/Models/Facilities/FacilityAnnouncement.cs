namespace Triumph.HealthMs.Core.Models.Facilities;

public class FacilityAnnouncement : FacilityEntity
{
    public OrganizationalFacility? OrganizationalFacility { get; set; }
    public string Message { get; set; } = string.Empty;
    public AnnouncementType Type { get; set; }
    public DateTime ValidUntil { get; set; }
}