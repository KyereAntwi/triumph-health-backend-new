namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class RemovePatientIdentificationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/patients/{id:guid}/identifications/{identificationId:guid}", Handle)
            .WithName("RemoveIdentification")
            .WithDescription("Remove a patient identification")
            .WithTags("Patients")
            .Produces<BaseResponse<string>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<string>>()
            .Produces<BaseResponse<string>>(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<FacilityIdRequired>()
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromRoute] Guid identificationId,
        [FromServices] ICommandHandler<RemoveIdentificationCommand, string> handler)
    {
        var command = new RemoveIdentificationCommand(id, identificationId);
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}