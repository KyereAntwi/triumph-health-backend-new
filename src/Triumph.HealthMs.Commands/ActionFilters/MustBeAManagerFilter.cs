namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class MustBeAManagerFilter(IPermissionService permissionService) 
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (await permissionService.UserIsAManager(CancellationToken.None))
        {
            return await next(context);
        }

        throw new UnauthorizedAccessException("Forbidden");
    }
}