namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetPermissionsConfigsHandler (
    IEmployeeManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext appUserContext)
    : IQueryHandler<object, IEnumerable<string>>
{
    public async Task<BaseResponse<IEnumerable<string>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var appUserId = await appUserContext
            .ApplicationUsers
            .Where(x => x.UserId == loggedInUserService.UserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        var employeePermissionIds = await dbContext
            .Employees
            .Select(e => new
            {
                e.ApplicationUserId,
                PermissionIds = e.EmployeePermissions.Select(ep => ep.PermissionId)
            })
            .Where(x => x.ApplicationUserId == appUserId)
            .SelectMany(x => x.PermissionIds)
            .ToArrayAsync(cancellationToken);
        
        if(employeePermissionIds.Length == 0)
            return new BaseResponse<IEnumerable<string>>
            {
                IsSuccess = true,
                Data = []
            };
        
        var permissions = await dbContext.Permissions
            .Where(p => ((IEnumerable<Guid>)employeePermissionIds).Contains(p.Id))
            .Select(p => p.PermissionType.ToString())
            .ToArrayAsync(cancellationToken);
        
        return new BaseResponse<IEnumerable<string>>
        {
            IsSuccess = true,
            Data = permissions
        };
    }
}