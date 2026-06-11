namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class UpdatePatientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/patients/{id:guid}", Handle)
            .WithName("UpdatePatient")
            .WithDescription("Update a patient")
            .WithTags("Patients")
            .Produces<BaseResponse<string>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<string>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<string>>()
            .Produces<BaseResponse<string>>(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .AddEndpointFilter<TenantIdRequiredFilter>()
            .AddEndpointFilter<RequiresActiveSubscription>()
            .AddEndpointFilter<FacilityIdRequired>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdatePatientCommand command,
        [FromServices] ICommandHandler<UpdatePatientCommand, string> handler)
    {
        command.PatientId = id;

        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}