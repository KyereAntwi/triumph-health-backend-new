namespace Triumph.HealthMs.Host.DI;

public static class ServicesStartup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "Triumph Health Management API";
                document.Info.Version = "v1";

                document.Components ??= new OpenApiComponents();
                
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                var authUrl = builder.Configuration["AuthServer:AuthorizationUrl"];
                var tokenUrl = builder.Configuration["AuthServer:TokenUrl"];
                
                if (!string.IsNullOrEmpty(authUrl) && !string.IsNullOrEmpty(tokenUrl))
                {
                    document.Components.SecuritySchemes["OAuth2"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(authUrl),
                                TokenUrl = new Uri(tokenUrl),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "OpenID Connect scope" },
                                    { "profile", "Profile scope" },
                                    { "email", "Email scope" },
                                    { "api.read", "API read access" }
                                }
                            }
                        }
                    };
                    
                    var schemeRef = new OpenApiSecuritySchemeReference("OAuth2", document);
                    document.Security ??= [];
                    
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        { schemeRef, ["openid", "profile", "email", "api.read"] }
                    });
                }
                
                return Task.CompletedTask;
            });
        });
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == "Development")
        {
            builder.Host.UseSerilog((_, _, configuration) =>
            {
                configuration
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
            });
        }
        else
        {
            builder.Host.UseSerilog((_, _, configuration) =>
            {
                configuration
                    .MinimumLevel.Debug()
                    .WriteTo.Console();
                
                // Only configure Sentry if DSN is provided
                var sentryDsn = builder.Configuration["Sentry:DSN"];
                if (!string.IsNullOrEmpty(sentryDsn))
                {
                    configuration.WriteTo.Sentry(options =>
                    {
                        options.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                        options.MinimumEventLevel = LogEventLevel.Warning;
                        options.Dsn = sentryDsn;
                        options.TracesSampleRate = 1.0;
                        options.EnableLogs = true;
                    });
                }
            });
        }
        
        // register layers
        builder.Services.AddExternalServicesLayer(builder.Configuration);

        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        
        builder.Services.AddProblemDetails();
        
        return builder.Build();
    }
}