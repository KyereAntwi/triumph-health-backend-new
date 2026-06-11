namespace Triumph.HealthMs.Commands.V1.EmployeeManagement;

public sealed class AddEmployeeShiftEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/employees/{id:guid}/shifts", Handle)
            .WithName("AddEmployeeShift")
            .WithDescription("Add an employee shift")
            .WithTags("Employees")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<FacilityIdRequired>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAFacilityManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] AddEmployeeShiftCommand command,
        [FromServices] ICommandHandler<AddEmployeeShiftCommand, Guid> handler)
    {
        command.EmployeeId = id;
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}