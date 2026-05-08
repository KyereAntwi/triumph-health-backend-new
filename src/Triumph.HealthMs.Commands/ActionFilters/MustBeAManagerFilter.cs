namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class MustBeAManagerFilter(
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
    ITenantManagementDbContext tenantManagementDbContext,
    IFacilityManagementDbContext facilityManagementDbContext) 
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

        if (string.IsNullOrEmpty(loggedInUserService.TenantId) &&
            string.IsNullOrEmpty(loggedInUserService.FacilityId)) 
            throw new UnauthorizedAccessException("Forbidden");

        if (!string.IsNullOrEmpty(loggedInUserService.TenantId))
        {
            var isTenantAManager = await tenantManagementDbContext
                .TenantManagers
                .AnyAsync(m =>
                    m.ApplicationUserId == existingUser!.Id && m.TenantId == Guid.Parse(loggedInUserService.TenantId));
            
            if (isTenantAManager)
            {
                return await next(context);
            }
        }

        if (string.IsNullOrEmpty(loggedInUserService.FacilityId)) throw new UnauthorizedAccessException("Forbidden");
        
        var isFacilityManager = await facilityManagementDbContext
            .FacilityManagers
            .AnyAsync(f =>
                f.ApplicationUserId == existingUser!.Id &&
                f.FacilityId == Guid.Parse(loggedInUserService.FacilityId));
        
        if (isFacilityManager)
        {
            return await next(context);
        }

        throw new UnauthorizedAccessException("Forbidden");
    }
}