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
                Subscriptions = x.TenantSubscriptions.Select(s => new
                {
                    s.Subscription!.Title,
                    s.SubscriptionChargeRate,
                    s.IsActive
                }),
                Managers = x.TenantManagers.Select(m => m.Id.ToString())
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
        
        innerQuery = innerQuery.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);

        var list = await innerQuery
            .Select(t => new TenantDto
            {
                Id = t.Id.ToString(),
                UniqueIdentifier = t.UniqueIdentifier,
                OrganizationTitle = t.OrganizationTitle,
                Email = t.Email,
                Address = t.Address,
                LogoUrl = t.LogoUrl ?? string.Empty,
                MainTelephone = t.MainTelephone,
                Subscriptions = t.Subscriptions.Select(s => new TenantSubscriptionDto(
                    Name: s.Title,
                    SubscriptionChargeRate: s.SubscriptionChargeRate.ToString(),
                    IsActive: s.IsActive)).ToList(),
                Managers = t.Managers.Select(m => new TenantManagerDto(m)).ToList()
            }).ToArrayAsync(cancellationToken);
        
        return new BaseResponse<IEnumerable<TenantDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}