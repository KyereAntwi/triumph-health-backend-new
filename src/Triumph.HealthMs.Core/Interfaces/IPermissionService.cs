namespace Triumph.HealthMs.Core.Interfaces;

public interface IPermissionService
{
    Task<bool> UserHasRequiredPermission(PermissionType permissionType, CancellationToken cancellationToken);
}