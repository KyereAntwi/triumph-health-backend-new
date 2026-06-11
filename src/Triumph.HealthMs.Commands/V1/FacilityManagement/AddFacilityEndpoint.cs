namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class AddFacilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/facilities", Handle)
            .WithName("AddFacility")
            .WithDescription("Register a facility for an Tenant Organization")
            .WithTags("Facilities")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeATenantManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddFacilityCommand command,
        [FromServices] ICommandHandler<AddFacilityCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult(
            routeName: "GetFacilityById",
            routeValues: new { Id = result.Data });
    }
}