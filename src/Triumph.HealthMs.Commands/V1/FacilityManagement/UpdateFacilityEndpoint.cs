namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class UpdateFacilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/facilities/{id:guid}", Handle)
            .WithName("UpdateFacility")
            .WithDescription("Update a facility details")
            .WithTags("Facilities")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status200OK)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAManagerFilter>();
    }
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdateFacilityCommand command,
        [FromServices] ICommandHandler<UpdateFacilityCommand, Guid> handler)
    {
        command.Id = id;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}