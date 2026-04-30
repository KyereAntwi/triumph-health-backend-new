namespace Triumph.HealthMs.Host.DI;

public static class PipelineStartup
{
    public static WebApplication AddPipelines(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
        
        app.MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet)
            .WithTags("Triumph Health API")
            .MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithSummary("Health Check")
            .WithDescription("Returns the health status of the API");

        SetupScalarDocumentation(app);
        
        return app;
    }
    
    static void SetupScalarDocumentation(WebApplication app)
    {
        // Use default OpenAPI mapping
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Triumph Health API")
                .WithTheme(ScalarTheme.Solarized)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            
            // Only add OAuth2 authentication if ClientId is configured
            var clientId = app.Configuration["AuthServer:ClientId"];
            if (!string.IsNullOrEmpty(clientId))
            {
#pragma warning disable CS0618
                options.WithOAuth2Authentication(opt =>
                {
                    opt.ClientId = clientId;
                    opt.Scopes = ["openid", "profile", "email", "api.read"];
                });
#pragma warning restore CS0618
                options.AddPreferredSecuritySchemes("OAuth2");
            }
        });
    }
}
