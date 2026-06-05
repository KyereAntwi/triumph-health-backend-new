namespace Triumph.HealthMs.Core.Models.Facilities;

public class OrganizationalFacility : TenantEntity
{
    public string UrlSuffix { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MainTelephone { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public DateOnly? EstablishedAt { get; set; }

    public ICollection<FacilityManager> FacilityManagers { get; set; } = [];
    public ICollection<FacilityAnnouncement> FacilityAnnouncements { get; set; } = [];
}