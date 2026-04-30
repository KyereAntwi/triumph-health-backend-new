namespace Triumph.HealthMs.ExternalServices.DI;

public static class RegisterExternalServicesLayer
{
    public static IServiceCollection AddExternalServicesLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.Audience = configuration["AuthServer:Audience"];
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"OnAuthenticationFailed {context.Exception}");
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumers(typeof(RegisterExternalServicesLayer).Assembly);
            
            var rabbitMqHost = configuration["RabbitMQ:Host"];
            if (string.IsNullOrEmpty(rabbitMqHost) || env == "Development")
            {
                config.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
            }
            else
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqHost, h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"] ??  "guest");
                        h.Password(configuration["RabbitMQ:Password"] ??   "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            }
        });
        
        return services;
    }
}