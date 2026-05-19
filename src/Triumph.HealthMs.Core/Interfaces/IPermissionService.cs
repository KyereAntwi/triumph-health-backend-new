namespace Triumph.HealthMs.Core.Interfaces;

public interface IPermissionService
{
    Task<bool> UserHasRequiredPermission(PermissionType permissionType, CancellationToken cancellationToken);
    Task<bool> HasActiveSubscription(CancellationToken cancellationToken);
}