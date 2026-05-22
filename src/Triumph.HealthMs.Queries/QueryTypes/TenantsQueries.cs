namespace Triumph.HealthMs.Queries.QueryTypes;

[ExtendObjectType<QueryBase>]
public class TenantsQueries
{
    [Authorize(Roles = ["SuperAdmin"])]
    public async Task<IEnumerable<TenantDto>> AllTenants(
        GetTenantsRequest? request,
        IResolverContext resolverContext,
        IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>> handler,
        CancellationToken cancellationToken)
    {
        var includeSubscriptions = resolverContext.IsSelected("subscriptions");
        var includeManagers = resolverContext.IsSelected("managers");

        var query = new GetTenantsQuery(
            TenantId: request?.TenantId ?? string.Empty,
            Identifier: request?.Identifier ?? string.Empty,
            SearchKey: request?.SearchKey ?? string.Empty,
            Page: request?.Page ?? 1,
            PageSize: request?.PageSize ?? 10,
            includeSubscriptions,
            includeManagers);
        
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.Data!;
    }

    [Authorize]
    public async Task<TenantDto> SingleTenant(
        IResolverContext resolverContext,
        IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>> handler,
        ILoggedInUserService loggedInUserService,
        IPermissionService permissionService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new GraphQLRequestException("Tenant Id missing");

        if (!await permissionService.HasActiveSubscription(cancellationToken))
            throw new GraphQLRequestException("You do not have an active subscription");

        if (!await permissionService.UserIsAManager(cancellationToken))
            throw new GraphQLRequestException("You are not authorized to access this resource");
        
        var includeSubscriptions = resolverContext.IsSelected("subscriptions");
        var includeManagers = resolverContext.IsSelected("managers");
        
        var query = new GetTenantsQuery(
            TenantId: loggedInUserService.TenantId,
            Identifier:  string.Empty,
            SearchKey:  string.Empty,
            Page: 1,
            PageSize: 1,
            IncludeSubscriptions: includeSubscriptions,
            IncludeManagers: includeManagers);

        var result = await handler
            .HandleAsync(query, cancellationToken);

        return result.Data!.Any() 
            ? result.Data!.First() 
            : throw new GraphQLRequestException("Tenant not found");
    }
    
    public async Task<IEnumerable<TenantFacilityDto>> Facilities(
        GetTenantFacilitiesRequest? request,
        IResolverContext context,
        IQueryHandler<GetTenantFacilitiesQuery, IEnumerable<TenantFacilityDto>> handler,
        CancellationToken cancellationToken)
    {
        var includeManagers = context.IsSelected("managers");

        var query = new GetTenantFacilitiesQuery(
            Page: request?.Page ?? 1,
            PageSize: request?.PageSize ?? 10,
            SearchKey: request?.SearchKey ?? string.Empty,
            TenantId: request?.TenantId ?? string.Empty,
            includeManagers);
        
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.Data!;
    }
}