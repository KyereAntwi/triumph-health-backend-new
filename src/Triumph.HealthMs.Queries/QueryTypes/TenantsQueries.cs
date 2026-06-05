using Triumph.HealthMs.Core.Features.TenantManagement.GetTenantAnnouncements;

namespace Triumph.HealthMs.Queries.QueryTypes;

[Authorize]
[ExtendObjectType<QueryBase>]
public class TenantsQueries(
    ICacheService cacheService,
    ILoggedInUserService loggedInUserService,
    IPermissionService permissionService)
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
    
    public async Task<TenantDto> SingleTenant(
        IResolverContext resolverContext,
        IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>> handler,
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
    
    [AllowAnonymous]
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
        
        var result = await cacheService.GetOrCreateAsync(
            CacheKeys.Facilities(),
            async token => await handler.HandleAsync(query, token),
            absoluteExpiry: TimeSpan.FromMinutes(30),
            cancellationToken);
        
        return result.Data!;
    }

    public async Task<IEnumerable<FacilityAnnouncementDto>> FacilityAnnouncements(
        GetFacilityAnnouncementsQuery? query,
        IQueryHandler<GetFacilityAnnouncementsQuery, IEnumerable<FacilityAnnouncementDto>> handler,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new GraphQLRequestException("Tenant Id missing");
        
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId))
            throw new GraphQLRequestException("Facility Id missing");

        if (!await permissionService.HasActiveSubscription(cancellationToken))
            throw new GraphQLRequestException("You do not have an active subscription");

        if (!await permissionService.UserIsAManager(cancellationToken))
            throw new GraphQLRequestException("You are not authorized to access this resource");
        
        var result = await handler.HandleAsync(query ?? new GetFacilityAnnouncementsQuery(), CancellationToken.None);
        return result.Data!;
    }

    public async Task<IEnumerable<TenantAnnouncementDto>> TenantAnnouncements(
        GetTenantAnnouncementsQuery? query,
        IQueryHandler<GetTenantAnnouncementsQuery, IEnumerable<TenantAnnouncementDto>> handler,
        CancellationToken cancellationToken)
    {
         if(!string.IsNullOrEmpty(loggedInUserService.TenantId))
             throw new GraphQLRequestException("Tenant Id missing");
         
         if(!await permissionService.HasActiveSubscription(cancellationToken))
             throw new GraphQLRequestException("You do not have an active subscription");
         
         if(!await permissionService.UserIsAManager(cancellationToken))
             throw new GraphQLRequestException("You are not authorized to access this resource");
         
         var result = await handler
             .HandleAsync(query ?? new GetTenantAnnouncementsQuery(), CancellationToken.None);
         return result.Data!;
    }
    
    public async Task<IEnumerable<DepartmentDto>> Departments(
        IQueryHandler<object, IEnumerable<DepartmentDto>> handler,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new GraphQLRequestException("Tenant Id missing");
        
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId))
            throw new GraphQLRequestException("Facility Id missing");

        if (!await permissionService.HasActiveSubscription(cancellationToken))
            throw new GraphQLRequestException("You do not have an active subscription");
        
        var result = await handler.HandleAsync(new object(), cancellationToken);
        return result.Data!;
    }
}