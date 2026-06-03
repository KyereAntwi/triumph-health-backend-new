namespace Triumph.HealthMs.Queries.QueryTypes;

[Authorize]
[ExtendObjectType<QueryBase>]
public class AppConfigurationQueries(
    ILoggedInUserService loggedInUserService,
    ICacheService cacheService)
{
    [GraphQLDescription("Get all configurations for the front end app.")]
    public async Task<ConfigsResponse> GetAppConfigurations(
        IResolverContext context,
        IQueryHandler<object, UserInformationDto> userConfigHandler,
        IQueryHandler<object, TenantInformationDto> tenantConfigHandler,
        IQueryHandler<object, FacilityInformationDto> facilityConfigHandler,
        IQueryHandler<object, RoleDto> roleConfigHandler,
        CancellationToken cancellationToken = default)
    {
        var loadUserProfile = context.IsSelected("userInformation");
        var loadTenantProfile = context.IsSelected("tenantInformation");
        var loadFacilityProfile = context.IsSelected("facilityInformation");
        var loadRoleProfile = context.IsSelected("roleInformation");

        var result = new ConfigsResponse();

        if (loadUserProfile)
        {
            var userConfigs =
                await cacheService.GetOrCreateAsync(
                    CacheKeys.UserProfile(loggedInUserService.UserId!),
                    async token => await userConfigHandler.HandleAsync(new object(), token),
                    absoluteExpiry: TimeSpan.FromDays(1),
                    cancellationToken);

            if (!userConfigs.IsSuccess)
                throw new GraphQLRequestException(userConfigs.Message);
            
            result.UserInformation = userConfigs.Data;
        }

        if (loadTenantProfile)
        {
            var tenantConfigs = 
                    await cacheService.GetOrCreateAsync(
                        CacheKeys.TenantProfile(loggedInUserService.UserId!),
                        async token => await tenantConfigHandler.HandleAsync(new object(), token),
                        absoluteExpiry: TimeSpan.FromDays(1),
                        cancellationToken);
            
            if (!tenantConfigs.IsSuccess)
                throw new GraphQLRequestException(tenantConfigs.Message);

            result.TenantInformation = tenantConfigs.Data;
        }

        if (loadFacilityProfile)
        {
            var facilityConfigs = 
                    await cacheService.GetOrCreateAsync(
                        CacheKeys.FacilityProfile(loggedInUserService.UserId!),
                        async token => await facilityConfigHandler.HandleAsync(new object(), token),
                        absoluteExpiry: TimeSpan.FromDays(1),
                        cancellationToken);
            
            if (!facilityConfigs.IsSuccess)
                throw new GraphQLRequestException(facilityConfigs.Message);

            result.FacilityInformation = facilityConfigs.Data;
        }

        if (loadRoleProfile)
        {
            var roleConfigs = 
                    await cacheService.GetOrCreateAsync(
                        CacheKeys.RoleProfile(loggedInUserService.UserId!),
                        async token => await roleConfigHandler.HandleAsync(new object(), token),
                        absoluteExpiry: TimeSpan.FromDays(1),
                        cancellationToken);
            
            if (!roleConfigs.IsSuccess)
                throw new GraphQLRequestException(roleConfigs.Message);

            result.RoleInformation = roleConfigs.Data;
        }

        return result;
    }
}