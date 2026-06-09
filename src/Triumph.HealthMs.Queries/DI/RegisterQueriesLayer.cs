namespace Triumph.HealthMs.Queries.DI;

public static class RegisterQueriesLayer
{
    public static IServiceCollection AddQueriesServices(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType(d => d.Field("hello").Resolve("World"));

        return services;
    }
}