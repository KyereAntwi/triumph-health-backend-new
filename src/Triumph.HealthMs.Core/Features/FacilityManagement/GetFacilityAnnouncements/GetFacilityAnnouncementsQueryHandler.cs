namespace Triumph.HealthMs.Core.Features.FacilityManagement.GetFacilityAnnouncements;

public sealed class GetFacilityAnnouncementsQueryHandler(IFacilityManagementDbContext dbContext) 
    : IQueryHandler<GetFacilityAnnouncementsQuery, IEnumerable<FacilityAnnouncementDto>>
{
    public async Task<BaseResponse<IEnumerable<FacilityAnnouncementDto>>> HandleAsync(GetFacilityAnnouncementsQuery query, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.FacilityAnnouncements
            .OrderByDescending(a => a.CreatedAt)
            .ThenBy(a => a.ValidUntil)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new FacilityAnnouncementDto(
                a.Id.ToString(),
                a.Message,
                a.Type.ToString(),
                a.ValidUntil.ToShortDateString()))
            .ToListAsync(cancellationToken);

        return new BaseResponse<IEnumerable<FacilityAnnouncementDto>>
        {
            IsSuccess = true,
            Data = result
        };
    }
}