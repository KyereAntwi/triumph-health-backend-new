namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class FacilityIdRequired(
    ILoggedInUserService loggedInUserService,
    IFacilityManagementDbContext facilityManagementDbContext) 
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId))
            throw new UnauthorizedAccessException("Facility Id missing");
        
        if (!await facilityManagementDbContext.OrganizationalFacilities.AnyAsync(f =>
                f.Id == Guid.Parse(loggedInUserService.FacilityId)))
            throw new UnauthorizedAccessException("No facility registered with provided Id");

        return await next(context);
    }
}