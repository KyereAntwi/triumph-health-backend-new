namespace Triumph.HealthMs.Core.DI;

public static class RegisterQueryCommandHandlers
{
    public static IServiceCollection AddQueryCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddAUserAccountCommand, Guid>, AddAUserAccountCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserInformationCommand, string>, UpdateUserInformationCommandHandler>();
        
        return services;
    }
}