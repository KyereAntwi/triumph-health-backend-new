namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class TenantIdRequiredFilter(
    ILoggedInUserService loggedInUserService,
    ITenantManagementDbContext tenantManagementDbContext) 
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new UnauthorizedAccessException("TenantId missing");
        
        if (!await tenantManagementDbContext.Tenants.AnyAsync(t =>
                t.Id == Guid.Parse(loggedInUserService.TenantId)))
            throw new UnauthorizedAccessException("No tenant registered with provided Id");
        
        return await next(context);
    }
}