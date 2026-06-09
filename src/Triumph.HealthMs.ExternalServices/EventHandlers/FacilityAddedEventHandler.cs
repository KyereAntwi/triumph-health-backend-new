namespace Triumph.HealthMs.ExternalServices.EventHandlers;

public sealed class FacilityAddedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDnsServices dnsServices,
    ISendMessage sendMessage,
    AppSettings appSettings,
    ILogger<FacilityAddedEventHandler> logger)
    : IConsumer<FacilityAddedEvent>
{
    public async Task Consume(ConsumeContext<FacilityAddedEvent> context)
    {
        logger.LogInformation("Received FacilityAddedEvent. Payload: {Payload}",  context.Message);

        var scope = serviceScopeFactory.CreateScope();
        var facilityDbContext = scope.ServiceProvider.GetRequiredService<IFacilityManagementDbContext>();

        var facility = await facilityDbContext.OrganizationalFacilities
            .IgnoreQueryFilters()
            .Where(f => f.Id == context.Message.FacilityId && !f.Deleted)
            .Select(f => new
            {
                f.UrlSuffix,
                f.Email,
                f.Name,
                f.TenantId,
                f.LogoUrl
            })
            .FirstOrDefaultAsync();

        if (facility is null)
        {
            logger.LogError("No facility found for FacilityAddedEventHandler with facility id: {FacilityId}", context.Message.FacilityId);
            return;
        }
        
        var tenantDbContext = scope.ServiceProvider.GetRequiredService<ITenantManagementDbContext>();
        var tenant = await tenantDbContext.Tenants
            .IgnoreQueryFilters()
            .Where(t => t.Id == facility.TenantId && !t.Deleted)
            .Select(t => new
            {
                t.OrganizationTitle
            })
            .FirstAsync();
        
        var response = await dnsServices.CreateSubdomain(facility.UrlSuffix);

        if (!response) return;

        const string subject = "Welcome to Triumph Health";
        var message = FacilityOnboardingTemplate.GetMessage(
            tenant.OrganizationTitle,
            facility.Name,
            facility.LogoUrl ?? string.Empty,
            $"{facility.UrlSuffix}.facilities-app.{appSettings.MainDomain}");
        
        await sendMessage.SendEmailAsync([facility.Email], null, subject, message);
    }
}