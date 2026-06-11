namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class AddFacilityManagerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/facilities/{id:guid}/managers", Handle)
            .WithName("AddFacilityManager")
            .WithDescription("Add a manager to a facility")
            .WithTags("Facilities")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<string>>(StatusCodes.Status409Conflict)
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
        [FromBody] AddFacilityManagerCommand command,
        [FromServices] ICommandHandler<AddFacilityManagerCommand, string> handler)
    {
        command.FacilityId = id;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}