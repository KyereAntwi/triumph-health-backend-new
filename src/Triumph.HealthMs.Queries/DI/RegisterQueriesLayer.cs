namespace Triumph.HealthMs.Queries.DI;

public static class RegisterQueriesLayer
{
    public static IServiceCollection AddQueriesServices(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<QueryBase>()
            .AddTypeExtension<HealthCheckQuery>()
            .AddTypeExtension<TenantsQueries>()
            .AddTypeExtension<EmployeesQueries>();
        
        return services;
    }
}