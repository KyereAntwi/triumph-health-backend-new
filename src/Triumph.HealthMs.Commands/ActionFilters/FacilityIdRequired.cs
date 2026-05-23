namespace Triumph.HealthMs.Commands.ActionFilters;

public sealed class FacilityIdRequired(ILoggedInUserService loggedInUserService) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId))
            throw new UnauthorizedAccessException("Facility Id missing");

        return await next(context);
    }
}