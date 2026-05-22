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
        IApplicationUserManagementDbContext applicationUserManagementDbContext)
    {
        if (context.Request.Path.StartsWithSegments("/favicon.svg") ||
            context.Request.Path.StartsWithSegments("/openapi") ||
            context.Request.Path.StartsWithSegments("/scalar") ||
            (context.Request.Method == HttpMethod.Get.ToString() && context.Request.Path.StartsWithSegments("/graphql")) ||
            (context.Request.Method == HttpMethod.Post.ToString() && context.Request.Path.StartsWithSegments("/api/v1/accounts")))
        {
            await _next(context);
            return;
        }

        if (!string.IsNullOrEmpty(loggedInUserService.UserId))
        {
            if (!await applicationUserManagementDbContext.ApplicationUsers.AnyAsync(a =>
                    a.UserId == loggedInUserService.UserId))
            {
                if (context.Request.Path.StartsWithSegments("/graphql"))
                    throw new GraphQLRequestException("No account registered for user");
                throw new UnauthorizedAccessException("No account registered for user");
            }
        }

        await _next(context);
    }
}