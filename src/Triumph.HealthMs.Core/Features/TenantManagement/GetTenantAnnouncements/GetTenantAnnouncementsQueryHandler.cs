namespace Triumph.HealthMs.Core.Features.TenantManagement.GetTenantAnnouncements;

public sealed class GetTenantAnnouncementsQueryHandler(
    ITenantManagementDbContext dbContext) 
    : IQueryHandler<GetTenantAnnouncementsQuery, IEnumerable<TenantAnnouncementDto>>
{
    public async Task<BaseResponse<IEnumerable<TenantAnnouncementDto>>> HandleAsync(GetTenantAnnouncementsQuery query, CancellationToken cancellationToken = default)
    {
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        
        var result = await dbContext
            .TenantAnnouncements
            .OrderByDescending(a => a.CreatedAt)
            .ThenBy(a => a.ValidUntil)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new TenantAnnouncementDto(
                Id: a.Id.ToString(),
                Message: a.Message,
                Type: a.Type.ToString(),
                ValidUntil: a.ValidUntil.ToString("o")))
            .ToArrayAsync<TenantAnnouncementDto>(cancellationToken);
        
        return new BaseResponse<IEnumerable<TenantAnnouncementDto>>
        {
            IsSuccess = true,
            Data = result
        };
    }
}