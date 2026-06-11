namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class UpdateTenantAnnouncementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("tenants/{id:guid}/announcements/{announcementId:guid}", Handle)
            .WithName("UpdateTenantAnnouncement")
            .WithDescription("Updates a tenant announcement.")
            .WithTags("Tenants")
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeATenantManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromRoute] Guid announcementId,
        [FromBody] UpdateTenantAnnouncementCommand command,
        [FromServices] ICommandHandler<UpdateTenantAnnouncementCommand, Guid> handler)
    {
        command.Id = id;
        command.AnnouncementId = announcementId;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}