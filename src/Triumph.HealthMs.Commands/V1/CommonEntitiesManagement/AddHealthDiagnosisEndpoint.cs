namespace Triumph.HealthMs.Commands.V1.CommonEntitiesManagement;

public sealed class AddHealthDiagnosisEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/health-diagnoses", Handle)
            .WithName("AddHealthDiagnosis")
            .WithTags("Common Entities")
            .Produces<BaseResponse<Guid>>()
            .Produces<BaseResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status403Forbidden)
            .Produces<BaseResponse<Guid>>(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromBody] AddHealthDiagnosisCommand command,
        [FromServices] ICommandHandler<AddHealthDiagnosisCommand, Guid> handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToHttpResult();
    }
}