namespace Triumph.HealthMs.Commands.V1.ApplicationUser;

public sealed class AddInvitationLinkEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/account-link-invitations", Handle)
            .WithName("AddLinkInvitation")
            .WithDescription("Add a link invitation for a user.")
            .WithTags("Account Link  Invitations")
            .Produces<BaseResponse<Guid>>()
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<MustBeAManagerFilter>();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddLinkInvitationCommand command, 
        [FromServices] ICommandHandler<AddLinkInvitationCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}