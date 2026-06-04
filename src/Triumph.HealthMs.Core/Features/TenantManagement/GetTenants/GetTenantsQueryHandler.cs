namespace Triumph.HealthMs.Core.Features.TenantManagement.GetTenants;

public sealed class GetTenantsQueryHandler(
    ITenantManagementDbContext dbContext) 
    : IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>>
{
    public async Task<BaseResponse<IEnumerable<TenantDto>>> HandleAsync(GetTenantsQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext
            .Tenants
            .OrderByDescending(x => x.CreatedAt)
            .IgnoreQueryFilters()
            .Where(t => !t.Deleted)
            .Select(x => new
            {
                x.Id,
                x.UniqueIdentifier,
                x.OrganizationTitle,
                x.Email,
                x.Address,
                x.LogoUrl,
                x.MainTelephone,
                x.CreatedAt
            });

        if (!string.IsNullOrEmpty(query.TenantId))
        {
            innerQuery = innerQuery.Where(t => t.Id == Guid.Parse(query.TenantId));
        }

        if (!string.IsNullOrEmpty(query.Identifier))
        {
            innerQuery = innerQuery.Where(t => t.UniqueIdentifier == query.Identifier);
        }

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            innerQuery = innerQuery.Where(t => t.OrganizationTitle.ToLower().Contains(query.SearchKey.ToLower()));
        }

        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        var tenants = await innerQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var tenantIds = tenants.Select(t => t.Id);
        
        Dictionary<Guid, List<TenantSubscriptionDto>>? subscriptions = null;
        if (query.IncludeSubscriptions)
        {
            subscriptions = await dbContext
                .TenantSubscriptions
                .Where(ts => tenantIds.Contains(ts.TenantId))
                .Select(s => new
                {
                    s.TenantId,
                    s.Subscription!.Title,
                    s.SubscriptionChargeRate,
                    s.IsActive
                })
                .GroupBy(x => x.TenantId)
                .ToDictionaryAsync(g => g.Key,
                    g => g.Select(x =>
                            new TenantSubscriptionDto(
                                x.Title,
                                x.SubscriptionChargeRate.ToString(),
                                x.IsActive))
                        .ToList(),
                    cancellationToken);
        }
        
        Dictionary<Guid, List <TenantManagerDto>> ? managers = null;
        if (query.IncludeManagers)
        {
            managers = await dbContext
                .TenantManagers
                .Where(tm => tenantIds.Contains((Guid)tm.TenantId!))
                .Select(m => new
                {
                    m.TenantId,
                    m.Id
                })
                .GroupBy(x => (Guid)x.TenantId!)
                .ToDictionaryAsync(g => g.Key, g => 
                    g.Select(x => new TenantManagerDto(Id: x.Id.ToString())).ToList(), 
                    cancellationToken);
        }

        var list = tenants
            .Select(t => new TenantDto
            {
                Id = t.Id.ToString(),
                UniqueIdentifier = t.UniqueIdentifier,
                OrganizationTitle = t.OrganizationTitle,
                Email = t.Email,
                Address = t.Address,
                LogoUrl = t.LogoUrl ?? string.Empty,
                MainTelephone = t.MainTelephone,
                
                Subscriptions =  query.IncludeSubscriptions && subscriptions != null && subscriptions.TryGetValue(t.Id, out var subLists) ? subLists : [],
                Managers = query.IncludeManagers && managers != null && managers.TryGetValue(t.Id, out var managersList) ? managersList : []
            });
        
        return new BaseResponse<IEnumerable<TenantDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}