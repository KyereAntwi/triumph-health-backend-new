namespace Triumph.HealthMs.Core.Models.Tenants;

public class Tenant : AuditableEntity
{
    public string UniqueIdentifier { get; set; } = string.Empty;
    
    // organizational properties for tenant
    public string OrganizationTitle { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string MainTelephone { get; set; } = string.Empty;

    public ICollection<TenantSubscription> TenantSubscriptions { get; set; } = [];
    public ICollection<TenantManager> TenantManagers { get; set; } = [];
    public ICollection<TenantAnnouncement> TenantAnnouncements { get; set; } = [];
}