namespace Triumph.HealthMs.Core.Features.FacilityManagement.GetFacilityAnnouncements;

public record GetFacilityAnnouncementsQuery(
    int Page = 1,
    int PageSize = 10);
    
public record FacilityAnnouncementDto(string Id, string Message, string Type, string ValidUntil);