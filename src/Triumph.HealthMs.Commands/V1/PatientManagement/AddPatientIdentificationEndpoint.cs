namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class AddPatientIdentificationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/patients/{id:guid}/identifications", Handle)
            .WithName("AddIdentification")
            .WithDescription("Add a patient identification")
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
        [FromBody] AddPatientIdentityCommand command,
        [FromServices] ICommandHandler<AddPatientIdentityCommand, Guid> handler)
    {
        command.PatientId = id;

        var result = await handler.HandleAsync(command);
        return result.ToHttpResult("GetIdentificationById", new { id = result.Data });
    }
}