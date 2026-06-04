namespace Triumph.HealthMs.Commands.V1.EmployeeManagement;

public sealed class StartAShiftEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/employees/{id:guid}/shifts/start", Handle)
            .WithName("StartEmployeeShift")
            .WithDescription("Start an employee shift")
            .WithTags("Employees")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<FacilityIdRequired>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<StartAShiftCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(new StartAShiftCommand(id));
        return result.ToHttpResult();
    }
}