namespace Triumph.HealthMs.Commands.V1.CommonEntitiesManagement;

public sealed class AddVitalItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/vital-items", Handle)
            .WithName("AddVitalItem")
            .WithTags("Common Entities")
            .Produces<BaseResponse<Guid>>()
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status403Forbidden)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .HasApiVersion(1);
    }

    private static async Task<IResult> Handle(
        [FromBody] AddVitalItemCommand command,
        [FromServices] ICommandHandler<AddVitalItemCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}