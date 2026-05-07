namespace Triumph.HealthMs.Core.Interfaces;

public interface ILoggedInUserService
{
    string? UserId { get; }
    string? TenantId { get; }
    string? FacilityId { get; }
}