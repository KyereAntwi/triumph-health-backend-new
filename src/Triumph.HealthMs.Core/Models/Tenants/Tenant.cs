namespace Triumph.HealthMs.Core.Models.Tenants;

public class Tenant : AuditableEntity
{
    public string UniqueIdentifier { get; set; } = string.Empty;

    public ICollection<TenantSubscription> TenantSubscriptions { get; set; } = [];
    public ICollection<TenantManager> TenantManagers { get; set; } = [];
}