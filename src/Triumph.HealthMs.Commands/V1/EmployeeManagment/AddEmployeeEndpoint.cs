namespace Triumph.HealthMs.Commands.V1.EmployeeManagment;

public sealed class AddEmployeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/employees", Handle)
            .WithName("AddEmployee")
            .WithDescription("Add an employee and send an invitation link")
            .WithTags("Employees")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<MustBeAManagerFilter>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddAnEmployeeCommand command,
        [FromServices] ICommandHandler<AddAnEmployeeCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult("GetEmployeeById", new { Id = result.Data });
    }
}