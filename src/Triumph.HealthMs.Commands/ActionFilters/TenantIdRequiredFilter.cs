namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class TenantIdRequiredFilter(ILoggedInUserService loggedInUserService) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new UnauthorizedAccessException("TenantId missing");
        
        return await next(context);
    }
}