namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetTenantConfigsQueryHandler(
    ITenantManagementDbContext dbContext,
    IFacilityManagementDbContext facilityManagementDbContext,
    IApplicationUserManagementDbContext userDbContext)
    : IQueryHandler<object, TenantInformationDto>
{
    public async Task<BaseResponse<TenantInformationDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;

        if (!string.IsNullOrEmpty(ctx.TenantId))
        {
            var innerQuery = await dbContext
                .Tenants
                .Where(t => t.Id == Guid.Parse(ctx.TenantId))
                .Select(t => new TenantInformationDto(
                    t.Id.ToString(),
                    t.OrganizationTitle,
                    t.LogoUrl ?? string.Empty,
                    t.Address,
                    t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
                .FirstOrDefaultAsync(cancellationToken);

            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = true,
                Data = innerQuery!
            };
        }

        if (!string.IsNullOrEmpty(ctx.FacilityUrlPrefix) || !string.IsNullOrEmpty(ctx.FacilityId))
        {
            var innerQuery = await facilityManagementDbContext.OrganizationalFacilities
                .Where(f =>
                    ctx.FacilityId != null && (f.Id == Guid.Parse(ctx.FacilityId) || ctx.FacilityUrlPrefix != null && (f.UrlSuffix == ctx.FacilityUrlPrefix)))
                .Select(f => f.TenantId)
                .FirstOrDefaultAsync(cancellationToken);
            
            var tenant = await dbContext.Tenants.Where(t => t.Id == innerQuery!)
                .Select(t => new TenantInformationDto(
                    t.Id.ToString(),
                    t.OrganizationTitle,
                    t.LogoUrl ?? string.Empty,
                    t.Address,
                    t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
                .FirstOrDefaultAsync(cancellationToken);
            
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = true,
                Data = tenant!
            };
        }
        
        // check for manager association
        var appUserId = await userDbContext.ApplicationUsers
            .Where(u => u.UserId == ctx.UserId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var managerResult = await dbContext.TenantManagers
            .Where(tm => tm.ApplicationUserId == appUserId!)
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