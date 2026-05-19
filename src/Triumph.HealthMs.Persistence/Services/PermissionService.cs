namespace Triumph.HealthMs.Persistence.Services;

public sealed class PermissionService (
    IEmployeeManagementDbContext dbContext, 
    IApplicationUserManagementDbContext appUserDbContext,
    ITenantManagementDbContext tenantManagementDbContext,
    ILoggedInUserService loggedInUserService)
    : IPermissionService
{
    public async Task<bool> UserHasRequiredPermission(PermissionType permissionType, CancellationToken cancellationToken)
    {
        var appUserId = await appUserDbContext.ApplicationUsers
            .Where(u => u.UserId == loggedInUserService.UserId)
            .Select(u => new { u.Id })
            .FirstAsync(cancellationToken);

        return await dbContext
            .Employees
            .Select(e => new
            {
                e.ApplicationUserId,
                Permissions = e.EmployeePermissions.Select(ep => ep.Permission!.PermissionType)
            })
            .AnyAsync(e => e.ApplicationUserId == appUserId.Id && e.Permissions.Contains(permissionType),
                cancellationToken);
    }

    public async Task<bool> HasActiveSubscription(CancellationToken cancellationToken)
    {
        return await tenantManagementDbContext
            .TenantSubscriptions
            .AnyAsync(ts => ts.TenantId == Guid.Parse(loggedInUserService.TenantId!) &&
                            ts.IsActive && 
                            ts.ExpiresAt.Date >= DateTime.UtcNow.Date, cancellationToken);
    }
}