namespace Triumph.HealthMs.Core.Features.TenantManagement.RenewSubscription;

public record TenantSubscriptionRenewedEvent : IntegrationEvent
{
    public Guid TenantSubscription { get; set; }
}