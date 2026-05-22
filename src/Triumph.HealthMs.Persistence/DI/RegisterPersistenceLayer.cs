using Marten;

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

        services.AddDbContext<ApplicationUserManagementDbContext>(ConfigureDbContext);
        services.AddScoped<IApplicationUserManagementDbContext>(sp => sp.GetRequiredService<ApplicationUserManagementDbContext>());

        services.AddDbContext<TenantManagementDbContext>(ConfigureDbContext);
        services.AddScoped<ITenantManagementDbContext>(sp => sp.GetRequiredService<TenantManagementDbContext>());

        services.AddDbContext<FacilityManagementDbContext>(ConfigureDbContext);
        services.AddScoped<IFacilityManagementDbContext>(sp => sp.GetRequiredService<FacilityManagementDbContext>());

        services.AddDbContext<EmployeeManagementDbContext>(ConfigureDbContext);
        services.AddScoped<IEmployeeManagementDbContext>(sp => sp.GetRequiredService<EmployeeManagementDbContext>());

        services.AddDbContext<PatientManagementDbContext>(ConfigureDbContext);
        services.AddScoped<IPatientManagementDbContext>(sp => sp.GetRequiredService<PatientManagementDbContext>());

        services.AddDbContext<CommonEntitiesDbContext>(ConfigureDbContext);
        services.AddScoped<ICommonEntitiesDbContext>(sp => sp.GetRequiredService<CommonEntitiesDbContext>());

        services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.DatabaseSchemaName = "audit";
            options.Schema.For<AuditLog>();
        })
        .UseLightweightSessions();

        services.AddScoped<IUpsetEmployeeService, UpsetEmployeeService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPatientUpsetService, PatientUpsetService>();

        return services;
        
        void ConfigureDbContext(
            IServiceProvider sp,
            DbContextOptionsBuilder options)
        {
            options.UseNpgsql(sp.GetRequiredService<NpgsqlConnection>(),
                    postgresOptions =>
                    {
                        postgresOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    })
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        }
    }
}
