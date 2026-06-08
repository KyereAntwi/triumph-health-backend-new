namespace Triumph.HealthMs.Queries.QueryTypes;

[ExtendObjectType<QueryBase>]
public class AppConfigurationQueries(
    ILoggedInUserService loggedInUserService,
    ICacheService cacheService)
{
    [Authorize]
    [GraphQLDescription("Get all configurations for the front end app.")]
    public async Task<ConfigsResponse> GetAppConfigurations(
        IResolverContext context,
        IQueryHandler<object, UserInformationDto> userConfigHandler,
        IQueryHandler<object, TenantInformationDto> tenantConfigHandler,
        IQueryHandler<object, FacilityInformationDto> facilityConfigHandler,
        IQueryHandler<object, RoleDto> roleConfigHandler,
        IQueryHandler<object, IEnumerable<string>> permissionsConfigHandler,
        IQueryHandler<object, IEnumerable<AnnouncementDto>> announcementConfigHandler,
        IQueryHandler<object, IEnumerable<UiStorageItemDto>> uiStorageItemsConfigHandler,
        CancellationToken cancellationToken = default)
    {
        var loadUserProfile = context.IsSelected("userInformation");
        var loadTenantProfile = context.IsSelected("tenantInformation");
        var loadFacilityProfile = context.IsSelected("facilityInformation");
        var loadRoleProfile = context.IsSelected("roleInformation");
        var loadAnnouncementProfile = context.IsSelected("announcements");
        var loadUiStorageItemsProfile = context.IsSelected("uiStorageItems");
        var loadPermissionsProfile = context.IsSelected("permissions");

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
        
        if(loadPermissionsProfile)
        {
            var permissionsConfigs = 
                    await cacheService.GetOrCreateAsync(
                        CacheKeys.PermissionsProfile(loggedInUserService.UserId!),
                        async token => await permissionsConfigHandler.HandleAsync(new object(), token),
                        absoluteExpiry: TimeSpan.FromDays(1),
                        cancellationToken);
            
            if (!permissionsConfigs.IsSuccess)
                throw new GraphQLRequestException(permissionsConfigs.Message);

            result.Permissions = permissionsConfigs.Data!;
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

        if (loadAnnouncementProfile)
        {
            var announcementConfigs = await cacheService.GetOrCreateAsync(
                CacheKeys.AnnouncementProfile(loggedInUserService.UserId!),
                async token => await announcementConfigHandler.HandleAsync(new object(), token),
                absoluteExpiry: TimeSpan.FromDays(1),
                cancellationToken);
            
            if (!announcementConfigs.IsSuccess)
                throw new GraphQLRequestException(announcementConfigs.Message);
            
            result.Announcements = announcementConfigs.Data!;
        }
        
        if(loadUiStorageItemsProfile)
        {
            var uiStorageItemsConfigs = await cacheService.GetOrCreateAsync(
                CacheKeys.UiStorageItemsProfile(loggedInUserService.UserId!),
                async token => await uiStorageItemsConfigHandler.HandleAsync(new object(), token),
                absoluteExpiry: TimeSpan.FromDays(1),
                cancellationToken);

            if (!uiStorageItemsConfigs.IsSuccess)
                throw new GraphQLRequestException(uiStorageItemsConfigs.Message);

            result.UiStorageItems = uiStorageItemsConfigs.Data!;
        }

        return result;
    }
}