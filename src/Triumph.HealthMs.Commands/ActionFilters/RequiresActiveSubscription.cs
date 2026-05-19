namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class RequiresActiveSubscription(
    IPermissionService permissionService) 
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!await permissionService.HasActiveSubscription(CancellationToken.None))
            throw new UnauthorizedAccessException("Subscription invalid");

        return await next(context);
    }
}