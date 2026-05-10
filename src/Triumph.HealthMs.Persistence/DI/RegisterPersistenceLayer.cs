namespace Triumph.HealthMs.Persistence.DI;

public static class RegisterPersistenceLayer
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditingInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException();

        // Shared connection so all tenant contexts can enlist in a single transaction
        services.AddScoped(_ => new NpgsqlConnection(connectionString));

        services.AddDbContext<ApplicationUserManagementDbContext>((sp, options) =>
        {
            options.UseNpgsql(sp.GetRequiredService<NpgsqlConnection>())
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });
        services.AddScoped<IApplicationUserManagementDbContext>(
            sp => sp.GetRequiredService<ApplicationUserManagementDbContext>());

        services.AddDbContext<TenantManagementDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString)
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });
        services.AddScoped<ITenantManagementDbContext>(
            sp => sp.GetRequiredService<TenantManagementDbContext>());

        services.AddDbContext<FacilityManagementDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(sp.GetRequiredService<NpgsqlConnection>())
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });
        services.AddScoped<IFacilityManagementDbContext>(
            sp => sp.GetRequiredService<FacilityManagementDbContext>());

        services.AddDbContext<EmployeeManagementDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(sp.GetRequiredService<NpgsqlConnection>())
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });
        services.AddScoped<IEmployeeManagementDbContext>(
            sp => sp.GetRequiredService<EmployeeManagementDbContext>());

        services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.DatabaseSchemaName = "audit";
            options.Schema.For<AuditLog>();
        })
        .UseLightweightSessions();

        services.AddScoped<IUpsetEmployeeService, UpsetEmployeeService>();

        return services;
    }
}
