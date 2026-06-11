namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class RemoveFacilityManagerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/facilities/{id:guid}/managers/{managerId:guid}", Handle)
            .WithName("RemoveFacilityManager")
            .WithDescription("Remove a manager from a facility")
            .WithTags("Facilities")
            .Produces<BaseResponse<string>>(StatusCodes.Status200OK)
            .Produces<BaseResponse<string>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromRoute] Guid managerId,
        [FromServices] ICommandHandler<RemoveFacilityManagerCommand, string> handler)
    {
        var result = await handler.HandleAsync(new RemoveFacilityManagerCommand
        {
            FacilityId = id,
            ManagerId = managerId
        });

        return result.ToHttpResult();
    }
}