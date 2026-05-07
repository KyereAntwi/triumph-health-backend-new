namespace Triumph.HealthMs.Core.DI;

public static class RegisterQueryCommandHandlers
{
    public static IServiceCollection AddQueryCommandHandlers(this IServiceCollection services)
    {
        #region ApplicationUserManagement
        services.AddScoped<ICommandHandler<AddAUserAccountCommand, Guid>, AddAUserAccountCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserInformationCommand, string>, UpdateUserInformationCommandHandler>();
        #endregion

        #region TenantManagment
        services.AddScoped<ICommandHandler<AddTenantAccountCommand, AddTenantAccountResponse>, AddTenantAccountCommandHandler>();
        services.AddScoped<ICommandHandler<RenewSubscriptionCommand, Guid>, RenewSubscriptionCommandHandler>();
        services.AddScoped<ICommandHandler<AddTenantManagerCommand, Guid>, AddTenantManagerCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveTenantManagerCommand, string>, RemoveTenantManagerCommandHandler>();
        #endregion
        
        return services;
    }
}