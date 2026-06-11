namespace Triumph.HealthMs.Commands.V1.FacilityManagement;

public sealed class UpdateFacilityAnnouncementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/facilities/{id:guid}/announcements/{announcementId:guid}", Handle)
            .WithName("UpdateFacilityAnnouncement")
            .WithDescription("Update a facility announcement")
            .WithTags("Facilities")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<string>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<string>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAFacilityManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromRoute] Guid announcementId,
        [FromBody] UpdateFacilityAnnouncementCommand command,
        [FromServices] UpdateFacilityAnnouncementCommandHandler handler)
    {
        command.Id = id;
        command.AnnouncementId = announcementId;
        
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}