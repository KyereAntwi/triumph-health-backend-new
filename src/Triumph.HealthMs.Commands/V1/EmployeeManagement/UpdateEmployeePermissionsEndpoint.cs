namespace Triumph.HealthMs.Commands.V1.EmployeeManagement;

public sealed class UpdateEmployeePermissionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/employees/{id:guid}/permissions", Handle)
            .WithName("UpdateEmployeePermissions")
            .WithDescription("Update an employee's permissions")
            .WithTags("Employees")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<string>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<string>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeAManagerFilter>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute]Guid id,
        [FromBody] UpdateEmployeePermissionsCommand command,
        [FromServices] UpdateEmployeePermissionsCommandHandler handler)
    {
        command.EmployeeId = id;

        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    } 
}