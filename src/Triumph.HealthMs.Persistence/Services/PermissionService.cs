namespace Triumph.HealthMs.Persistence.Services;

public class PermissionService (
    IEmployeeManagementDbContext dbContext, 
    IApplicationUserManagementDbContext appUserDbContext,
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
}