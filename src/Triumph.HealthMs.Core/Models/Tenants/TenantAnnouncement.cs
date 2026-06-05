namespace Triumph.HealthMs.Core.Models.Tenants;

public class TenantAnnouncement : TenantEntity
{
    public Tenant? Tenant { get; set; }
    public string Message { get; set; } = string.Empty;
    public AnnouncementType Type { get; set; }
    public DateTime ValidUntil { get; set; }
}