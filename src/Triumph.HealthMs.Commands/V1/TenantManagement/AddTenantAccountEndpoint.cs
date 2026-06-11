namespace Triumph.HealthMs.Commands.V1.TenantManagement;

public sealed class AddTenantAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tenants", Handle)
            .WithName("AddTenantAccount")
            .WithDescription("Adds a new tenant account to the system.")
            .WithTags("Tenants")
            .Produces<BaseResponse<AddTenantAccountResponse>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1);
    }

    private static async Task<IResult> Handle(
        [FromBody] AddTenantAccountCommand command,
        [FromServices] ICommandHandler<AddTenantAccountCommand, AddTenantAccountResponse> handle)
    {
        var result = await handle.HandleAsync(command);
        return result.ToHttpResult(
            routeName: "GetTenantByIdentifier",
            routeValues: result is { IsSuccess: true, Data: not null } ? 
                new { Id = result.Data!.TenantId } : 
                null);
    }
}