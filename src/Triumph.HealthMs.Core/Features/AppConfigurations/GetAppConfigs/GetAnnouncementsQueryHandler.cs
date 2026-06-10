namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetAnnouncementsQueryHandler(
    ITenantManagementDbContext tenantDbContext,
    IFacilityManagementDbContext facilityDbContext)
    : IQueryHandler<object, IEnumerable<AnnouncementDto>>
{
    public async Task<BaseResponse<IEnumerable<AnnouncementDto>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;
        List<AnnouncementDto> announcements = [];

        if (string.IsNullOrEmpty(ctx.FacilityId) &&
            string.IsNullOrEmpty(ctx.FacilityUrlPrefix))
            return new BaseResponse<IEnumerable<AnnouncementDto>>
            {
                IsSuccess = false,
                Message = "Facility prefix or facility id is missing"
            };
        
        var facilityAnnouncements = await facilityDbContext.FacilityAnnouncements
            .Where(a => a.ValidUntil >= DateTime.UtcNow)
            .Select(a => new AnnouncementDto(
                a.Id.ToString(),
                a.Message,
                a.Type.ToString(),
                "Facility"))
            .ToListAsync(cancellationToken);
        
        announcements.AddRange(facilityAnnouncements);
        
        
        var tenantAnnouncements = await tenantDbContext.TenantAnnouncements
            .Where(a => a.ValidUntil >= DateTime.UtcNow)
            .Select(a => new AnnouncementDto(
                a.Id.ToString(),
                a.Message,
                a.Type.ToString(),
                "Tenant"))
            .ToListAsync(cancellationToken);

        announcements.AddRange(tenantAnnouncements);

        return new BaseResponse<IEnumerable<AnnouncementDto>>
        {
            IsSuccess = true,
            Data = announcements
        };
    }
}