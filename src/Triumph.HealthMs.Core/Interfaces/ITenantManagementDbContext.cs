namespace Triumph.HealthMs.Core.Interfaces;

public interface ITenantManagementDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<TenantSubscription> TenantSubscriptions { get; }
    DbSet<TenantManager> TenantManagers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}