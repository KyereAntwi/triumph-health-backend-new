namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class AddTenantManagerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tenants/{id:guid}/managers", Handle)
            .WithName("AddTenantManager")
            .WithDescription("Add a manager to a tenant account")
            .WithTags("Tenants")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeATenantManagerFilter>();

    }

    private static async Task<IResult> Handle(
        [FromBody] AddTenantManagerCommand command,
        [FromServices] ICommandHandler<AddTenantManagerCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}