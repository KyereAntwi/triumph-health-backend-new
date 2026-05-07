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
        ITenantManagementDbContext tenantManagementDbContext,
        IApplicationUserManagementDbContext applicationUserManagementDbContext)
    {
        if (context.Request.Path.StartsWithSegments("/favicon.svg") ||
            context.Request.Path.StartsWithSegments("/openapi") ||
            context.Request.Path.StartsWithSegments("/scalar") ||
            context.Request.Path.StartsWithSegments("/graphql") ||
            (context.Request.Method == HttpMethod.Post.ToString() && context.Request.Path.StartsWithSegments("/api/v1/accounts")))
        {
            await _next(context);
            return;
        }

        if (!string.IsNullOrEmpty(loggedInUserService.UserId))
        {
            if (!await applicationUserManagementDbContext.ApplicationUsers.AnyAsync(a =>
                    a.UserId == loggedInUserService.UserId))
                throw new UnauthorizedAccessException("No account registered for user");
        }

        if (!string.IsNullOrEmpty(loggedInUserService.TenantId))
        {
            if (!await tenantManagementDbContext.Tenants.AnyAsync(t =>
                    t.Id == Guid.Parse(loggedInUserService.TenantId)))
                throw new UnauthorizedAccessException("No tenant registered with provided Id");
        }

        if (!string.IsNullOrEmpty(loggedInUserService.FacilityId))
        {
            // TODO - check for facility availability
        }

        await _next(context);
    }
}