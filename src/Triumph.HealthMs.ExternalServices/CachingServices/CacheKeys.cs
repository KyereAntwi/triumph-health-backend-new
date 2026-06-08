namespace Triumph.HealthMs.ExternalServices.CachingServices;

public static class CacheKeys
{
    // app configurations
    public static string UserProfile(string userId) => $"userProfile:{userId}";
    public static string TenantProfile(string userId) => $"tenantProfile:{userId}";
    public static string FacilityProfile(string userId) => $"facilityProfile:{userId}";
    public static string RoleProfile(string userId) => $"roleProfile:{userId}";
    public static string AnnouncementProfile(string userId) => $"announcementProfile:{userId}";
    public static string PermissionsProfile(string userId) => $"permissionsProfile:{userId}";
    public static string UiStorageItemsProfile(string userId) => $"uiStorageItemsProfile:{userId}";
    public static string Facilities() => "facilities";
    public static string Subscriptions() => "subscriptions";
}