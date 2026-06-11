namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetUserRoleConfigsQueryHandler(
    IApplicationUserManagementDbContext userDbContext,
    IEmployeeManagementDbContext employeeDbContext,
    IPatientManagementDbContext patientDbContext,
    ITenantManagementDbContext tenantDbContext,
    IFacilityManagementDbContext facilityDbContext)
    : IQueryHandler<object, RoleDto>
{
    public async Task<BaseResponse<RoleDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;
        var appUserId = await userDbContext.ApplicationUsers
            .Where(u => ctx.UserId == u.UserId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var isAPatient = await patientDbContext.Patients
            .AnyAsync(p => p.ApplicationUserId == appUserId!, cancellationToken);

        if (isAPatient)
        {
            return new BaseResponse<RoleDto>
            {
                IsSuccess = true,
                Data = new RoleDto(
                    nameof(RoleEntityType.Patient),
                    string.Empty,
                    false,
                    false)
            };
        }

        var isATenantManager = await tenantDbContext.TenantManagers
            .AnyAsync(t => t.ApplicationUserId == appUserId, cancellationToken);

        if (isATenantManager && string.IsNullOrEmpty(ctx.FacilityUrlPrefix))
        {
            return new BaseResponse<RoleDto>
            {
                IsSuccess = true,
                Data = new RoleDto(
                    nameof(RoleEntityType.TenantManager),
                    string.Empty,
                    true,
                    false)
            };
        }

        var isAFacilityManager = await facilityDbContext.FacilityManagers
            .AnyAsync(f => f.ApplicationUserId == appUserId, cancellationToken);

        var employeeRole = await employeeDbContext.Employees
            .Where(e => e.ApplicationUserId == appUserId)
            .Select(e => e.EmployeeRoles
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => r.Role!.Title)
                .LastOrDefault())
            .FirstOrDefaultAsync(cancellationToken);

        return isAFacilityManager switch
        {
            true when employeeRole is null => new BaseResponse<RoleDto>
            {
                IsSuccess = true,
                Data = new RoleDto(nameof(RoleEntityType.FacilityManager), string.Empty, false, true)
            },
            false when employeeRole is not null => new BaseResponse<RoleDto>
            {
                IsSuccess = true,
                Data = new RoleDto(nameof(RoleEntityType.FacilityEmployee), employeeRole, false, false)
            },
            true when true => new BaseResponse<RoleDto>
            {
                IsSuccess = true,
                Data = new RoleDto(nameof(RoleEntityType.FacilityEmployee), employeeRole, false, true)
            },
            _ => new BaseResponse<RoleDto> { IsSuccess = false, Message = "Role configurations not found" }
        };
    }
}