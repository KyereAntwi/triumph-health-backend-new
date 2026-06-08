namespace Triumph.HealthMs.Commands.V1.ApplicationUser;

public sealed class AddUiStorageItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/ui-storage-items", Handle)
            .WithName("AddUiStorageItem")
            .WithDescription("Adds a new UI storage item for the logged-in user.")
            .WithTags("Application Users")
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<RequiresActiveSubscription>()
            .HasApiVersion(1)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddAUiStorageItemCommand command,
        [FromServices] ICommandHandler<AddAUiStorageItemCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}