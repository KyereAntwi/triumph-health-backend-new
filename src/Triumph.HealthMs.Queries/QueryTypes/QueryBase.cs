namespace Triumph.HealthMs.Queries.QueryTypes;

public class QueryBase
{
    public async Task<IEnumerable<SubscriptionDto>> Subscriptions(
        IQueryHandler<GetAllSubscriptionsQuery, IEnumerable<SubscriptionDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAllSubscriptionsQuery(), cancellationToken);
        return result.Data!;
    }

    [Authorize]
    public async Task<IEnumerable<PermissionDto>> EmployeePermissions(
        IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAllPermissionsQuery(), cancellationToken);
        return result.Data!;
    }

    [Authorize]
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