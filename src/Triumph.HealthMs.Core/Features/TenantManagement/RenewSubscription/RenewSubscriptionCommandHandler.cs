namespace Triumph.HealthMs.Core.Features.TenantManagement.RenewSubscription;

public sealed class RenewSubscriptionCommandHandler(
    ILoggedInUserService loggedInUserService,
    ITenantManagementDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<RenewSubscriptionCommandHandler> logger) 
    : ICommandHandler<RenewSubscriptionCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(RenewSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new RenewSubscriptionCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Error",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        
        var existingSubscription = await dbContext.Subscriptions.FindAsync(Guid.Parse(command.SubscriptionId), cancellationToken);
        if (existingSubscription is null)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Subscription was not found"]
            };
        }

        var newTenantSubscription = new TenantSubscription
        {
            Id = Guid.CreateVersion7(),
            SubscriptionId = Guid.Parse(command.SubscriptionId),
            SubscriptionChargeRate = Enum.Parse<SubscriptionChargeRate>(command.SubscriptionChargeRate),
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(
                CalculateMonthsUntilSubscriptionExpires.Calculate(
                    Enum.Parse<SubscriptionChargeRate>(command.SubscriptionChargeRate)))
        };

        await dbContext.TenantSubscriptions.AddAsync(newTenantSubscription, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await PublishSubscriptionRenewedEvent(newTenantSubscription.Id);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Subscription renewed successfully",
            Data = newTenantSubscription.Id
        };
    }
    
    private async Task PublishSubscriptionRenewedEvent(Guid tenantSubId)
    {
        var @event = new TenantSubscriptionRenewedEvent
        {
            TenantSubscription = tenantSubId,
            UserId = loggedInUserService.UserId!,
            Action = "Tenant Account Created",
            EntityName = nameof(TenantSubscription),
            EntityId = tenantSubId
        };

        try
        {
            await publishEndpoint.Publish(@event);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Problem publishing TenantSubscriptionRenewedEvent. Payload = {Payload}", @event);
        }
    }
}