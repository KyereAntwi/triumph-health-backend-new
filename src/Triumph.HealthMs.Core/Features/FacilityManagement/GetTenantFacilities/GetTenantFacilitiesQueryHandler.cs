namespace Triumph.HealthMs.Core.Features.FacilityManagement.GetTenantFacilities;

public sealed class GetTenantFacilitiesQueryHandler(
    IFacilityManagementDbContext dbContext) 
    : IQueryHandler<GetTenantFacilitiesQuery, IEnumerable<TenantFacilityDto>>
{
    public async Task<BaseResponse<IEnumerable<TenantFacilityDto>>> HandleAsync(GetTenantFacilitiesQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext
            .OrganizationalFacilities
            .IgnoreQueryFilters()
            .Where(f => !f.Deleted)
            .Select(of => new
            {
                of.Id,
                of.Name,
                of.Address,
                of.Email,
                of.LogoUrl,
                of.MainTelephone,
                of.Description,
                of.EstablishedAt,
                of.TenantId
            });

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            innerQuery = innerQuery.Where(of =>
                of.Name.Contains(query.SearchKey) ||
                of.Address.Contains(query.SearchKey));
        }

        if (!string.IsNullOrEmpty(query.TenantId))
        {
            innerQuery = innerQuery.Where(of => of.TenantId == Guid.Parse(query.TenantId));
        }
        
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        var facilities = await innerQuery
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        
        Dictionary<Guid, List<string>>? managers = null;
        if (query.IncludeManagers)
        {
            var facilitiesId = facilities.Select(f => f.Id);

            managers = await dbContext
                .FacilityManagers
                .Where(fm => facilitiesId.Contains((Guid)fm.FacilityId!))
                .Select(f => new
                {
                    f.FacilityId,
                    ManagerId = f.Id.ToString()
                })
                .GroupBy(x => (Guid)x.FacilityId!)
                .ToDictionaryAsync(g => g.Key,
                    g => g.Select(x => x.ManagerId).ToList(),
                    cancellationToken);
        }
        
        var list = facilities
            .Select(of => new TenantFacilityDto(
            of.Id.ToString(),
            of.Name,
            of.Address,
            of.Email,
            of.LogoUrl ?? string.Empty,
            of.MainTelephone,
            of.Description ?? string.Empty,
            of.EstablishedAt.ToString()!)
            {
                Managers = query.IncludeManagers && managers != null && managers.TryGetValue(of.Id, out var m) ? m : []
            });

        return new BaseResponse<IEnumerable<TenantFacilityDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}