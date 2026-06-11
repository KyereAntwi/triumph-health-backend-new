namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class AddTenantAnnouncementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("tenants/{id:guid}/announcements", Handle)
            .WithName("AddTenantAnnouncement")
            .WithDescription("Adds a tenant announcement.")
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
        [FromBody] AddTenantAnnouncementCommand command,
        [FromServices] ICommandHandler<AddTenantAnnouncementCommand, Guid> handler)
    {
        command.TenantId = id;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}