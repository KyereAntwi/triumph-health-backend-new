namespace Triumph.HealthMs.Commands.V1.ApplicationUser;

public sealed class LinkUserToAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/account-link-invitations/link", Handle)
            .WithName("LinkUserToAccount")
            .WithDescription("Link user to existing account.")
            .WithTags("Account Link  Invitations")
            .Produces<BaseResponse<Guid>>()
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1);
    }

    private static async Task<IResult> Handle(
        [FromBody] LinkUserToExistingAccountCommand command,
        [FromServices] ICommandHandler<LinkUserToExistingAccountCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}