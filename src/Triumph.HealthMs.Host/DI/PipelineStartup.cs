namespace Triumph.HealthMs.Host.DI;

public static class PipelineStartup
{
    public static WebApplication AddPipelines(this WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost
        });
        
        SetupScalarDocumentation(app);
        
        app.UseHttpsRedirection();

        app.UseExceptionHandler();
        app.UseCors("SecurePolicy");
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseMiddleware<UserResourceResolverMiddleware>();
        app.UseAuthorization();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
            
        app.MapGraphQL();
            
        var versionedGroup = app.MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);
        versionedGroup.MapCarter();
        
        return app;
    }

    private static void SetupScalarDocumentation(WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Triumph Health API")
                .WithTheme(ScalarTheme.Solarized)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            
            var clientId = app.Configuration["AuthServer:ClientId"];
            var clientSecret = app.Configuration["AuthServer:ClientSecret"];
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)) return;
            
            options.AddPreferredSecuritySchemes("OAuth2");
            options.AddAuthorizationCodeFlow("OAuth2", flow =>
            {
                flow.ClientId = clientId;
                flow.ClientSecret = clientSecret;
                flow.Pkce = Pkce.Sha256;
                flow.SelectedScopes = ["openid", "profile", "email", "api.read"];
                flow.AuthorizationUrl = app.Configuration["AuthServer:AuthorizationUrl"];
                flow.TokenUrl = app.Configuration["AuthServer:TokenUrl"];
                
                flow.AddQueryParameter("audience", app.Configuration["AuthServer:Audience"]!);
            });
        });
    }
}