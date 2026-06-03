namespace Triumph.HealthMs.ExternalServices.DI;

public static class RegisterExternalServicesLayer
{
    public static IServiceCollection AddExternalServicesLayer(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
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
                    NameClaimType = "name",
                    RoleClaimType = "https://qa.triumphhealthms.com/roles/roles"
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>

                    {

                        var token = context.Token ??

                                    context.Request.Headers.Authorization.ToString();

                        Console.WriteLine($"TOKEN: {token}");

                        return Task.CompletedTask;

                    },
                    
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"OnAuthenticationFailed {context.Exception}");
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumers(typeof(PatientAddedEventHandler).Assembly);
            
            var rabbitMqHost = configuration["RabbitMQ:HostName"];
            if (string.IsNullOrEmpty(rabbitMqHost) || environment.IsDevelopment())
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
        
        if (environment.IsDevelopment())
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        }
        else
        {
            var redisConnection = configuration.GetConnectionString("Redis") 
                                  ?? throw new InvalidOperationException("Redis connection string is required.");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "hms:";
            });

            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 512;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(10),
                    LocalCacheExpiration = TimeSpan.FromMinutes(2)
                };
            });

            services.AddSingleton<ICacheService, HybridCacheService>();
        }

        services.AddHttpClient("resend", client =>
        {
            client.BaseAddress = new Uri(configuration["Resend:BaseUrl"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Resend:ApiKey"]}");
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Triumph Health Management System");
        });

        services.AddHttpClient("arkessel", client =>
        {
            client.BaseAddress = new Uri(configuration["Arkessel:BaseUrl"]!);
            client.DefaultRequestHeaders.Add("api-key", configuration["Arkessel:ApiKey"]);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        });

        
        services.AddTransient<ISendMessage, MessagingServices>();
        
        services.AddSingleton<ResendSettings>();
        services.Configure<ResendSettings>(configuration.GetSection("Resend"));
        
        services.AddSingleton<ArkesselSettings>();
        services.Configure<ArkesselSettings>(configuration.GetSection("Arkessel"));
        
        return services;
    }
}