namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;

public record TenantAccountAddedEvent : IntegrationEvent
{
    public Guid TenantSubscription { get; set; }
}