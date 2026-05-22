namespace Triumph.HealthMs.Core.Features.TenantManagement.GetTenants;

public record GetTenantsQuery(
    string TenantId,
    string Identifier,
    string SearchKey,
    int Page,
    int PageSize,
    bool IncludeSubscriptions,
    bool IncludeManagers);

public record TenantSubscriptionDto(string Name, string SubscriptionChargeRate, bool IsActive);

public record TenantManagerDto(string Id);

public record TenantDto
{
    public string Id { get; set; } = string.Empty;
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string OrganizationTitle { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string MainTelephone { get; set; } = string.Empty;
    public ICollection<TenantSubscriptionDto> Subscriptions { get; set; } = [];
    public ICollection<TenantManagerDto> Managers { get; set; } = [];
}

public record GetTenantsRequest(
    string TenantId = "",
    string Identifier = "",
    string SearchKey = "",
    int Page = 1,
    int PageSize = 10);