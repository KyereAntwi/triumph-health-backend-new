namespace Triumph.HealthMs.Commands.V1.EmployeeManagement;

public sealed class EndAShiftEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/employees/{id:guid}/shifts/end", Handle)
            .WithName("EndEmployeeShift")
            .WithDescription("End an employee shift")
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
        [FromServices] ICommandHandler<EndAShiftCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(new EndAShiftCommand(id));
        return result.ToHttpResult();
    }
}