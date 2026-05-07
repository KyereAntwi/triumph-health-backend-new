namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class RemoveTenantManagerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/tenants/{id:guid}/managers/{tenantManagerId:guid}", Handle)
            .WithName("RemoveTenantManager")
            .WithDescription("Remove an existing manager from a tenant")
            .WithTags("Tenants")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<MustBeATenantManagerFilter>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid tenantManagerId,
        [FromServices] ICommandHandler<RemoveTenantManagerCommand, string> handler)
    {
        var result = await handler.HandleAsync(new RemoveTenantManagerCommand(tenantManagerId));
        return result.ToHttpResult();
    }
}