namespace Triumph.HealthMs.Core.Models.Tenants;

public class TenantManager : TenantEntity
{
    public Tenant? Tenant { get; set; }

    public Guid ApplicationUserId { get; set; }
}