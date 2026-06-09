namespace Triumph.HealthMs.ExternalServices.EventHandlers;

public sealed class TenantAccountAddedEventHandler(
    ILogger<TenantAccountAddedEventHandler> logger,
    IServiceScopeFactory scopeFactory,
    ISendMessage sendMessage) 
    : IConsumer<TenantAccountAddedEvent>
{
    public async Task Consume(ConsumeContext<TenantAccountAddedEvent> context)
    {
        logger.LogInformation("Received TenantAccountAddedEvent for TenantId: {TenantId}. Payload: {Event}", context.Message.EntityId, context.Message);

        var scope = scopeFactory.CreateScope();
        var tenantDbContext = scope.ServiceProvider.GetRequiredService<ITenantManagementDbContext>();

        var tenant = await tenantDbContext.Tenants
            .Where(t => !t.Deleted && t.Id == context.Message.EntityId)
            .Select(t => new
            {
                t.OrganizationTitle,
                t.Email,
                t.UniqueIdentifier,
                Subscription = t.TenantSubscriptions.Select(ts => new
                {
                    ts.Subscription!.Title,
                    ts.ExpiresAt,
                    ts.SubscriptionChargeRate
                }).First()
            })
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            logger.LogError("Error occured processing event. Error = Tenant with ID {TenantId} not found in the database. Event body {Event}", context.Message.EntityId, context.Message);
            return;
        }

        var emailBody = TenantOnboardingTemplate.GetMessage(
            tenant.OrganizationTitle,
            tenant.UniqueIdentifier,
            tenant.Subscription.Title,
            tenant.Subscription.ExpiresAt,
            tenant.Subscription.SubscriptionChargeRate);

        await sendMessage.SendEmailAsync(
            [tenant.Email],
            null,
            "Welcome to Triumph Health",
            emailBody);
    }
}