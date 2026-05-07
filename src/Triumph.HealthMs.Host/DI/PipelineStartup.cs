namespace Triumph.HealthMs.Host.DI;

public static class PipelineStartup
{
    public static WebApplication AddPipelines(this WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
        });
        
        SetupScalarDocumentation(app);
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
        
        var versionedGroup = app.MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);
        
        versionedGroup.MapCarter();
        
        return app;
    }
    
    static void SetupScalarDocumentation(WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Triumph Health API")
                .WithTheme(ScalarTheme.Solarized)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            
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
