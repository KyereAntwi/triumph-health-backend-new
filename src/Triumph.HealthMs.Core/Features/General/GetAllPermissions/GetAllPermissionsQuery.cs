namespace Triumph.HealthMs.Core.Features.General.GetAllPermissions;

public record GetAllPermissionsQuery;

public record PermissionDto(string Id, string PermissionType, string DisplayName, string Description);