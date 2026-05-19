namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class AddVisitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/patients/{id:guid}/visitations", Handle)
            .WithName("AddVisitation")
            .WithDescription("Add a visitation")
            .WithTags("Patients")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<FacilityIdRequired>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id, 
        [FromBody] AddVisitCommand command,
        [FromServices] ICommandHandler<AddVisitCommand, Guid> handler)
    {
        command.PatientId = id;

        var result = await handler.HandleAsync(command);
        return result.ToHttpResult("GetVisitationById", new { id = result.Data });
    }
}