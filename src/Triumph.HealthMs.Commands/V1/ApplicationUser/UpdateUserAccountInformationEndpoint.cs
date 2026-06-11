namespace Triumph.HealthMs.Commands.V1.ApplicationUser;

public sealed class UpdateUserAccountInformationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/accounts", Handle)
            .WithName("UpdateUserAccountInformation")
            .WithDescription("Update the information of the currently logged in user's account")
            .WithTags("Application Users")
            .Produces<BaseResponse<string>>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1);
    }

    private static async Task<IResult> Handle([FromBody] UpdateUserInformationCommand command, [FromServices] ICommandHandler<UpdateUserInformationCommand, string> handle)
    {
        var result = await handle.HandleAsync(command);
        return result.ToHttpResult();
    }
}