namespace Triumph.HealthMs.Persistence.DI;

public static class RegisterPersistenceLayer
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditingInterceptor>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException();

        services.AddScoped<IApplicationUserManagementDbContext, ApplicationUserManagementDbContext>();
        services.AddDbContext<ApplicationUserManagementDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditingInterceptor>());
        });
        
        services.AddScoped<ITenantManagementDbContext, TenantManagementDbContext>();
        services.AddDbContext<TenantManagementDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IFacilityManagementDbContext, FacilityManagementDbContext>();
        services.AddDbContext<FacilityManagementDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(connectionString)
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });

        services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.DatabaseSchemaName = "audit";
            options.Schema.For<AuditLog>();
        })
        .UseLightweightSessions();
        
        return services;
    }
}