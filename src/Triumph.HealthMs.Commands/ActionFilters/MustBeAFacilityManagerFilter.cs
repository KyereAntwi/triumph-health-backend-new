namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class MustBeAFacilityManagerFilter (
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
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