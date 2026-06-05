namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class AddFacilityAnnouncementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/facilities/{id:guid}/announcements", Handle)
            .WithName("AddFacilityAnnouncement")
            .WithDescription("Add a facility announcement")
            .WithTags("Facilities")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<string>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAFacilityManagerFilter>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] AddFacilityAnnouncementCommand command,
        [FromServices] ICommandHandler<AddFacilityAnnouncementCommand, Guid> handler)
    {
        command.FacilityId = id;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}