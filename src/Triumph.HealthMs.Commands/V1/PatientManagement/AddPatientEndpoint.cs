using Triumph.HealthMs.Core.Features.PatientManagement.AddPatient;

namespace Triumph.HealthMs.Commands.V1.PatientManagement;

public sealed class AddPatientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/patients", Handle)
            .WithName("AddPatient")
            .WithDescription("Add a patient and send an invitation link")
            .WithTags("Patients")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status404NotFound)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .HasApiVersion(1)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddPatientCommand command,
        [FromServices] ICommandHandler<AddPatientCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult(
            routeName: "GetPatientById",
            routeValues: new
            {
                id = result.Data
            });
    }
}