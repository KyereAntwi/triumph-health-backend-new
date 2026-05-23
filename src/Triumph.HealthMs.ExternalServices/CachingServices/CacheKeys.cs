namespace Triumph.HealthMs.ExternalServices.CachingServices;

public static class CacheKeys
{
    // app configurations
    public static string UserProfile(string userId) => $"userProfile:{userId}";
    public static string TenantProfile(string userId) => $"tenantProfile:{userId}";
    public static string FacilityProfile(string userId) => $"facilityProfile:{userId}";
    public static string RoleProfile(string userId) => $"roleProfile:{userId}";
}