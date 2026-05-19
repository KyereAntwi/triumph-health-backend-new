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
                of.TenantId,
                ManagersIds = of.FacilityManagers.Select(m => m.ApplicationUserId.ToString())
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
        
        innerQuery = innerQuery.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
        var list = await innerQuery.Select(of => new TenantFacilityDto(
            of.Id.ToString(),
            of.Name,
            of.Address,
            of.Email,
            of.LogoUrl ?? string.Empty,
            of.MainTelephone,
            of.Description ?? string.Empty,
            of.EstablishedAt.ToString()!,
            of.ManagersIds))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<TenantFacilityDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}