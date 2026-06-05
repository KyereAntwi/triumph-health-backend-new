namespace Triumph.HealthMs.Core.Features.TenantManagement.GetTenantAnnouncements;

public record GetTenantAnnouncementsQuery(
    int Page = 1,
    int PageSize = 10);
    
public record TenantAnnouncementDto(string Id, string Message, string Type, string ValidUntil);