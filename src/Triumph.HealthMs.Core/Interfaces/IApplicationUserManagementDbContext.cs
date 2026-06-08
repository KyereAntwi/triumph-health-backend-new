namespace Triumph.HealthMs.Core.Interfaces;

public interface IApplicationUserManagementDbContext
{
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<LinkInvitation> LinkInvitations { get; }
        DbSet<UiStorageItem> UiStorageItems { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}