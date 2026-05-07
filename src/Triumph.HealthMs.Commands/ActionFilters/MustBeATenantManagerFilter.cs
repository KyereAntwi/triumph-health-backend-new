namespace Triumph.HealthMs.Commands.ActionFilters;

public class MustBeATenantManagerFilter(
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
    ITenantManagementDbContext tenantManagementDbContext)
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var existingUser = await applicationUserManagementDbContext
            .ApplicationUsers
            .Select(a => new
            {
                a.UserId,
                a.Id
            })
            .FirstOrDefaultAsync(a => a.UserId == loggedInUserService.UserId);
        
        var isAManager = await tenantManagementDbContext
            .TenantManagers
            .AnyAsync(m => m.ApplicationUserId == existingUser!.Id && m.TenantId == Guid.Parse(loggedInUserService.TenantId!));
        
        if (isAManager)
        {
            return await next(context);
        }

        throw new UnauthorizedAccessException("Forbidden");
    }
}