namespace Triumph.HealthMs.Core.DI;

public static class RegisterQueryCommandHandlers
{
    public static IServiceCollection AddQueryCommandHandlers(this IServiceCollection services)
    {
        services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        return services;
    }
}