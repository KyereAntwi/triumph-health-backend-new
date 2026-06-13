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
            return await TenantIdExists(dbContext, ctx, cancellationToken);
        } 
        
        if(!string.IsNullOrEmpty(ctx.FacilityUrlPrefix) || !string.IsNullOrEmpty(ctx.FacilityId))
        {
            return await FacilityInformationExists(dbContext, facilityManagementDbContext, ctx, cancellationToken);
        }
        
        return await IsATenantManager(dbContext, userDbContext, ctx, cancellationToken);
    }

    private static async Task<BaseResponse<TenantInformationDto>> IsATenantManager(
        ITenantManagementDbContext dbContext,
        IApplicationUserManagementDbContext userDbContext,
        AppConfigUserContext ctx,
        CancellationToken cancellationToken)
    {
        var appUserId = await userDbContext.ApplicationUsers
            .Where(u => u.UserId == ctx.UserId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (appUserId is null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = false,
                Message = "User account not found"
            };
        } 
        
        var managerResult = await dbContext.TenantManagers
            .Where(tm => tm.ApplicationUserId == appUserId)
            .Select(tm => new
            {
                tm.Tenant!.Id,
                tm.Tenant.OrganizationTitle,
                tm.Tenant.LogoUrl,
                tm.Tenant.Address,
                HasValidSubscriptions = tm.Tenant.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (managerResult is null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = false,
                Message = "Tenant configs not found",
                Data = null
            };
        }
        
        return new BaseResponse<TenantInformationDto>
        {
            IsSuccess = true,
            Data = new TenantInformationDto(
                managerResult.Id.ToString(), 
                managerResult.OrganizationTitle, 
                managerResult.LogoUrl ?? string.Empty, 
                managerResult.Address,
                managerResult.HasValidSubscriptions)
        };
    }

    private static async Task<BaseResponse<TenantInformationDto>> FacilityInformationExists(ITenantManagementDbContext dbContext,
        IFacilityManagementDbContext facilityManagementDbContext,
        AppConfigUserContext ctx,
        CancellationToken cancellationToken)
    {
        var innerQuery = await facilityManagementDbContext.OrganizationalFacilities
            .Where(f =>
                ctx.FacilityId != null && (f.Id == Guid.Parse(ctx.FacilityId) || ctx.FacilityUrlPrefix != null && (f.UrlSuffix == ctx.FacilityUrlPrefix)))
            .Select(f => f.TenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (innerQuery is null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = false,
                Message = "Facility not found"
            };
        }
        
        var tenant = await dbContext.Tenants.Where(t => t.Id == innerQuery)
            .Select(t => new TenantInformationDto(
                t.Id.ToString(),
                t.OrganizationTitle,
                t.LogoUrl ?? string.Empty,
                t.Address,
                t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = false,
                Message = "Tenant not found"
            };
        }

        return new BaseResponse<TenantInformationDto>
        {
            IsSuccess = true,
            Data = tenant
        };
    }

    private static async Task<BaseResponse<TenantInformationDto>> TenantIdExists(
        ITenantManagementDbContext dbContext,
        AppConfigUserContext ctx,
        CancellationToken cancellationToken)
    {
        var innerQuery = await dbContext
            .Tenants
            .Where(t => t.Id == Guid.Parse(ctx.TenantId!))
            .Select(t => new TenantInformationDto(
                t.Id.ToString(),
                t.OrganizationTitle,
                t.LogoUrl ?? string.Empty,
                t.Address,
                t.TenantSubscriptions.Any(ts => ts.IsActive && ts.ExpiresAt >= DateTime.UtcNow)))
            .FirstOrDefaultAsync(cancellationToken);

        if (innerQuery is null)
        {
            return new BaseResponse<TenantInformationDto>
            {
                IsSuccess = false,
                Message = "Tenant not found"
            };
        }

        return  new BaseResponse<TenantInformationDto>
        {
            IsSuccess = true,
            Data = innerQuery
        };
    }
}