namespace Triumph.HealthMs.Commands.V1.ApplicationUser;

public sealed class AddUserAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/accounts", Handle)
            .WithName("AddUserAccount")
            .WithDescription("Adds a new user account to the system.")
            .WithTags("ApplicationUsers")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle([FromBody] AddAUserAccountCommand  command, [FromServices] ICommandHandler<AddAUserAccountCommand, Guid> handle)
    {
        var result = await handle.HandleAsync(command);
        return result.ToHttpResult(
            routeName: "GetUserAccountById",
            routeValues: new { Id = result.Data });
    }
}