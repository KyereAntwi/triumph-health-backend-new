namespace Triumph.HealthMs.Core.Features.TenantManagement.GetTenants;

public record GetTenantsQuery(
    string TenantId = "",
    string Identifier = "",
    string SearchKey = "",
    int Page = 1,
    int PageSize = 10);

public record TenantSubscriptionDto(string Name, string SubscriptionChargeRate, bool IsActive);

public record TenantManagerDto(string Id);

public record TenantDto
{
    public string Id { get; set; }
    public string UniqueIdentifier { get; set; }
    public string OrganizationTitle { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string LogoUrl { get; set; }
    public string MainTelephone { get; set; } = string.Empty;
    public ICollection<TenantSubscriptionDto> Subscriptions { get; set; } = [];
    public ICollection<TenantManagerDto> Managers { get; set; } = [];
}