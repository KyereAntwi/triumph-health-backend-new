namespace Triumph.HealthMs.Core.Features.General.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler(IEmployeeManagementDbContext dbContext) :
    IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>
{
    public async Task<BaseResponse<IEnumerable<PermissionDto>>> HandleAsync(GetAllPermissionsQuery query, CancellationToken cancellationToken = default)
    {
        var list = await dbContext.Permissions.Select(p => new PermissionDto(
                Id: p.Id.ToString(),
                PermissionType: p.PermissionType.ToString(),
                DisplayName: p.DisplayName,
                Description: p.Description))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<PermissionDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}