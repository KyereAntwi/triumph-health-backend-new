namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetTenantConfigsQueryHandler(
    ITenantManagementDbContext dbContext,
    IFacilityManagementDbContext facilityManagementDbContext,
    IApplicationUserManagementDbContext userDbContext,
    ILoggedInUserService loggedInUserService) 
    : IQueryHandler<object, TenantInformationDto>
{
    public async Task<BaseResponse<TenantInformationDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(loggedInUserService.TenantId))
        {
            var innerQuery = await dbContext
                .Tenants
                .Where(t => t.Id == Guid.Parse(loggedInUserService.TenantId))
                .Select(t => new TenantInformationDto(
                    t.Id.ToString(),
                    t.OrganizationTitle,
                    t.LogoUrl ?? string.Empty,
                    t.Address,
                    t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
                .FirstAsync(cancellationToken);

            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = true,
                Data = innerQuery
            };
        }

        if (!string.IsNullOrEmpty(loggedInUserService.FacilityUrlPrefix) || !string.IsNullOrEmpty(loggedInUserService.FacilityId))
        {
            var innerQuery = await facilityManagementDbContext.OrganizationalFacilities
                .Where(f => 
                    loggedInUserService.FacilityId != null && (f.Id == Guid.Parse(loggedInUserService.FacilityId) || loggedInUserService.FacilityUrlPrefix != null && (f.UrlSuffix == loggedInUserService.FacilityUrlPrefix)))
                .Select(f => f.TenantId)
                .FirstAsync(cancellationToken);
            
            var tenant = await dbContext.Tenants.Where(t => t.Id == innerQuery)
                .Select(t => new TenantInformationDto(
                    t.Id.ToString(),
                    t.OrganizationTitle,
                    t.LogoUrl ?? string.Empty,
                    t.Address,
                    t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
                .FirstAsync(cancellationToken);
            
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = true,
                Data = tenant
            };
        }
        
        // check for manager association
        var appUserId = await userDbContext.ApplicationUsers
            .Where(u => u.UserId == loggedInUserService.UserId)
            .Select(u => u.Id)
            .FirstAsync(cancellationToken);

        var managerResult = await dbContext.TenantManagers
            .Where(tm => tm.ApplicationUserId == appUserId)
            .Select(tm => tm.Tenant)
            .FirstOrDefaultAsync(cancellationToken);

        if (managerResult is not null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = true,
                Data = new TenantInformationDto(
                    managerResult.Id.ToString(), 
                    managerResult.OrganizationTitle, 
                    managerResult.LogoUrl ?? string.Empty, 
                    managerResult.Address,
                    managerResult.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow))
            };
        }
        
        return new BaseResponse<TenantInformationDto>
        {
            IsSuccess = false,
            Message = "Tenant configs not found",
            Data = null
        };
    }
}