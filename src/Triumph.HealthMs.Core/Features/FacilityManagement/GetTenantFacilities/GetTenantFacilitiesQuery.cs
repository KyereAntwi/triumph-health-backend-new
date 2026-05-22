namespace Triumph.HealthMs.Core.Features.FacilityManagement.GetTenantFacilities;

public record GetTenantFacilitiesQuery(
    int Page,
    int PageSize,
    string SearchKey,
    string TenantId,
    bool IncludeManagers);

public record TenantFacilityDto(
    string Id,
    string Name,
    string Address,
    string Email,
    string LogoUrl,
    string MainTelephone,
    string Description,
    string EstablishedAt)
{
    public IEnumerable<string> Managers { get; set; } = [];
}

public record GetTenantFacilitiesRequest(
    int Page = 1,
    int PageSize = 10,
    string SearchKey = "",
    string TenantId = "");