namespace Triumph.HealthMs.Queries.QueryTypes;

public class QueryBase
{
    [GraphQLDescription("Get all subscriptions.")]
    public async Task<IEnumerable<SubscriptionDto>> Subscriptions(
        IQueryHandler<GetAllSubscriptionsQuery, IEnumerable<SubscriptionDto>> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(new GetAllSubscriptionsQuery(), cancellationToken);
        return result.Data!;
    }

    [Authorize]
    [GraphQLDescription("Get all permissions in the system.")]
    public async Task<IEnumerable<PermissionDto>> EmployeePermissions(
        IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAllPermissionsQuery(), cancellationToken);
        return result.Data!;
    }

    [Authorize]
    [GraphQLDescription("Get all drugs in the system.")]
    public async Task<IEnumerable<DrugDto>> Drugs(
        GetAllDrugsQuery? query,
        IQueryHandler<GetAllDrugsQuery, IEnumerable<DrugDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            query ?? new GetAllDrugsQuery(), 
            cancellationToken);
        
        return result.Data!;
    }
    
    [Authorize]
    [GraphQLDescription("Get all Health Diagnoses in the system.")]
    public async Task<IEnumerable<HealthDiagnosisDto>> HealthDiagnoses(
        GetAllHealthDiagnosisQuery? query,
        IQueryHandler<GetAllHealthDiagnosisQuery, IEnumerable<HealthDiagnosisDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            query ?? new GetAllHealthDiagnosisQuery(), 
            cancellationToken);
        
        return result.Data!;
    }

    [Authorize]
    [GraphQLDescription("Get all lab tests in the system.")]
    public async Task<IEnumerable<LabTestDto>> LabTests(
        GetAllLabTestsQuery? query,
        IQueryHandler<GetAllLabTestsQuery, IEnumerable<LabTestDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            query ?? new GetAllLabTestsQuery(),
            cancellationToken);

        return result.Data!;
    }

    [Authorize]
    [GraphQLDescription("Get all vital items that can be measured about a patient.")]
    public async Task<IEnumerable<VitalItemDto>> VitalItems(
        GetAllVitalsQuery? query,
        IQueryHandler<GetAllVitalsQuery, IEnumerable<VitalItemDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            query ?? new GetAllVitalsQuery(),
            cancellationToken);

        return result.Data!;
    }
}