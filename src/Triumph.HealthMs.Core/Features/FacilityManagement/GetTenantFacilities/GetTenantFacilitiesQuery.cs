namespace Triumph.HealthMs.Core.Features.FacilityManagement.GetTenantFacilities;

public record GetTenantFacilitiesQuery(
    int Page,
    int PageSize,
    string SearchKey,
    string TenantId);

public record TenantFacilityDto(
    string Id,
    string Name,
    string Address,
    string Email,
    string LogoUrl,
    string MainTelephone,
    string Description,
    string EstablishedAt,
    IEnumerable<string> FacilityManagerIds);