namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetAnnouncementsQueryHandler(
    ITenantManagementDbContext tenantDbContext,
    IFacilityManagementDbContext facilityDbContext,
    ILoggedInUserService loggedInUserService) 
    : IQueryHandler<object, IEnumerable<AnnouncementDto>>
{
    public async Task<BaseResponse<IEnumerable<AnnouncementDto>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        List<AnnouncementDto> announcements = [];
        
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId) &&
            string.IsNullOrEmpty(loggedInUserService.FacilityUrlPrefix))
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