namespace Triumph.HealthMs.Core.Features.TenantManagement.RemoveTenantManager;

public record RemoveTenantManagerCommand(
    Guid TenantManagerId);