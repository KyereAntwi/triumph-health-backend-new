namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class RenewSubscriptionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tenants/{id:guid}/subscriptions", Handle)
            .WithName("RenewSubscription")
            .WithDescription("Renew a tenants subscription")
            .WithTags("Tenants")
            .Produces<BaseResponse<Guid>>()
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<MustBeATenantManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromBody] RenewSubscriptionCommand command,
        [FromServices] ICommandHandler<RenewSubscriptionCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}