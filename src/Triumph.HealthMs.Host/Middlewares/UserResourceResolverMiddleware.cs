namespace Triumph.HealthMs.Host.Middlewares;

public class UserResourceResolverMiddleware
{
    private readonly RequestDelegate _next;

    public UserResourceResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context, 
        ILoggedInUserService loggedInUserService,
        IApplicationUserManagementDbContext applicationUserManagementDbContext,
        ITenantManagementDbContext tenantManagementDbContext,
        IFacilityManagementDbContext facilityManagementDbContext)
    {
        if (context.User.Identity is not { IsAuthenticated: true } || 
            (context.Request.Method.Equals("POST", StringComparison.CurrentCultureIgnoreCase) &&
            context.Request.Path.StartsWithSegments("/api/v1/accounts")))
        {
            await _next(context);
            return;
        }

        if (!await applicationUserManagementDbContext.ApplicationUsers
                .AnyAsync(a => a.UserId == loggedInUserService.UserId))
        {
            if (context.Request.Path.StartsWithSegments("/graphql"))
                throw new GraphQLRequestException("No account registered for user");
            throw new UnauthorizedAccessException("No account registered for user");
        }

        if (!string.IsNullOrEmpty(loggedInUserService.TenantId))
        {
            if (!await tenantManagementDbContext.Tenants.AnyAsync(t =>
                    t.Id == Guid.Parse(loggedInUserService.TenantId)))
            {
                if (context.Request.Path.StartsWithSegments("/graphql"))
                    throw new GraphQLRequestException("No tenant account found for this tenant");
                throw new UnauthorizedAccessException("No tenant account found for this tenant");
            }
        }

        if (!string.IsNullOrEmpty(loggedInUserService.FacilityId))
        {
            if (!await facilityManagementDbContext.OrganizationalFacilities.AnyAsync(f =>
                    f.Id == Guid.Parse(loggedInUserService.FacilityId)))
            {
                if (context.Request.Path.StartsWithSegments("/graphql"))
                    throw new GraphQLRequestException("No facility account found for this facility");
                throw new UnauthorizedAccessException("No facility account found for this facility");
            }
        }

        await _next(context);
    }
}