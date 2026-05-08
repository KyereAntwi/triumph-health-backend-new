namespace Triumph.HealthMs.Core.DI;

public static class RegisterQueryCommandHandlers
{
    public static IServiceCollection AddQueryCommandHandlers(this IServiceCollection services)
    {
        #region ApplicationUserManagement
        services.AddScoped<ICommandHandler<AddAUserAccountCommand, Guid>, AddAUserAccountCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserInformationCommand, string>, UpdateUserInformationCommandHandler>();
        services
            .AddScoped<ICommandHandler<LinkUserToExistingAccountCommand, Guid>,
                LinkUserToExistingAccountCommandHandler>();
        #endregion

        #region TenantManagment
        services.AddScoped<ICommandHandler<AddTenantAccountCommand, AddTenantAccountResponse>, AddTenantAccountCommandHandler>();
        services.AddScoped<ICommandHandler<RenewSubscriptionCommand, Guid>, RenewSubscriptionCommandHandler>();
        services.AddScoped<ICommandHandler<AddTenantManagerCommand, Guid>, AddTenantManagerCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveTenantManagerCommand, string>, RemoveTenantManagerCommandHandler>();
        #endregion

        #region FacilityManagement
        services.AddScoped<ICommandHandler<AddFacilityCommand, Guid>, AddFacilityCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateFacilityCommand, string>, UpdateFacilityCommandHandler>();
        services.AddScoped<ICommandHandler<AddFacilityManagerCommand, string>, AddFacilityManagerCommandHandler>();
        services
            .AddScoped<ICommandHandler<RemoveFacilityManagerCommand, string>, RemoveFacilityManagerCommandHandler>();
        #endregion
        
        return services;
    }
}