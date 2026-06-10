namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public record AppConfigUserContext(
    string? UserId,
    string? TenantId,
    string? FacilityId,
    string? FacilityUrlPrefix);
