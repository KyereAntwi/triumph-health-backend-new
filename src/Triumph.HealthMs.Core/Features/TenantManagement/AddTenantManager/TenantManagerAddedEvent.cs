namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantManager;

public record TenantManagerAddedEvent(
    Guid TenantManagerId) : IntegrationEvent;