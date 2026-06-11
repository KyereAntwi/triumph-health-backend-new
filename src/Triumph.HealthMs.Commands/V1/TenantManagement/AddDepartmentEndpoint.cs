namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class AddDepartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/departments", Handle)
            .WithName("AddDepartment")
            .WithDescription("Adds a department.")
            .WithTags("Tenants")
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<MustBeATenantManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddDepartmentCommand command, 
        [FromServices] ICommandHandler<AddDepartmentCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}