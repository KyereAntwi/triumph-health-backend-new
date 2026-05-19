namespace Triumph.HealthMs.Queries.QueryTypes;

[ExtendObjectType<QueryBase>]
public class TenantsQueries
{
    [Authorize(Roles = ["SuperAdmin"])]
    public async Task<IEnumerable<TenantDto>> AllTenants(
        GetTenantsQuery query,
        IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.Data!;
    }

    [Authorize]
    public async Task<TenantDto> SingleTenant(
        IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>> handler,
        ILoggedInUserService loggedInUserService,
        IPermissionService permissionService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new GraphQLRequestException("Tenant Id missing");

        if (!await permissionService.HasActiveSubscription(cancellationToken))
            throw new GraphQLRequestException("You do not have an active subscription");

        var result = await handler.HandleAsync(new GetTenantsQuery(
            TenantId: loggedInUserService.TenantId,
            PageSize: 1), cancellationToken);

        return result.Data!.First();
    }
    
    public async Task<IEnumerable<TenantFacilityDto>> Facilities(
        GetTenantFacilitiesQuery query,
        IQueryHandler<GetTenantFacilitiesQuery, IEnumerable<TenantFacilityDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.Data!;
    }
}