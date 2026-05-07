namespace Triumph.HealthMs.Core.Interfaces;

public interface ITenantManagementDbContext
{
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<LinkInvitation> LinkInvitations { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}