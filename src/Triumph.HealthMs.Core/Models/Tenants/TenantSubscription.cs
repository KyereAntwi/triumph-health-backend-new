namespace Triumph.HealthMs.Core.Models.Tenants;

public class TenantSubscription : AuditableEntity
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public Guid SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    public SubscriptionChargeRate SubscriptionChargeRate { get; set; } = SubscriptionChargeRate.Monthly;
    public bool IsActive { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
}