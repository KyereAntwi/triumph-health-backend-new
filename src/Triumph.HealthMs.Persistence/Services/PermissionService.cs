namespace Triumph.HealthMs.Persistence.Services;

public sealed class PermissionService (
    IEmployeeManagementDbContext dbContext, 
    IApplicationUserManagementDbContext appUserDbContext,
    ITenantManagementDbContext tenantManagementDbContext,
    IFacilityManagementDbContext facilityManagementDbContext,
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

    public async Task<bool> UserIsAManager(CancellationToken cancellationToken)
    {
        var existingUser = await appUserDbContext
            .ApplicationUsers
            .Select(a => new
            {
                a.UserId,
                a.Id
            })
            .FirstOrDefaultAsync(a => a.UserId == loggedInUserService.UserId, cancellationToken);
        
        if (!string.IsNullOrEmpty(loggedInUserService.TenantId))
        {
            var isTenantAManager = await tenantManagementDbContext
                .TenantManagers
                .AnyAsync(m =>
                    m.ApplicationUserId == existingUser!.Id && m.TenantId == Guid.Parse(loggedInUserService.TenantId),
                    cancellationToken);
            
            if (isTenantAManager)
            {
                return true;
            }
        }

        if (string.IsNullOrEmpty(loggedInUserService.FacilityId)) return false;
        
        var isFacilityManager = await facilityManagementDbContext
            .FacilityManagers
            .AnyAsync(f =>
                f.ApplicationUserId == existingUser!.Id &&
                f.FacilityId == Guid.Parse(loggedInUserService.FacilityId),
                cancellationToken);
        
        return isFacilityManager;
    }
}