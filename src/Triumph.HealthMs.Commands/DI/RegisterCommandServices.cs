namespace Triumph.HealthMs.Commands.DI;

public static class RegisterCommandServices
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddCarter(null, conf =>
        {
            var modules = typeof(RegisterCommandServices).Assembly.GetTypes()
                .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
            
            var withModuleMethod = typeof(CarterConfigurator).GetMethod(nameof(CarterConfigurator.WithModule))!;
            
            foreach (var module in modules)
                withModuleMethod.MakeGenericMethod(module).Invoke(conf, null);
        });
        
        return services;
    }
}