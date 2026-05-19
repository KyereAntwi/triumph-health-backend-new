namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class TakeVitalMeasurementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/patients/{id:guid}/visitations/{visitationId:guid}/vital-measurements", Handle)
            .WithName("TakeVitals")
            .WithDescription("Take vital measurement")
            .WithTags("Patients")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
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
        [FromRoute] Guid visitationId,
        [FromBody] TakeVitalMeasurementCommand command,
        [FromServices] ICommandHandler<TakeVitalMeasurementCommand, string> handler)
    {
        command.VisitationId = visitationId;
        command.PatientId = id;

        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}