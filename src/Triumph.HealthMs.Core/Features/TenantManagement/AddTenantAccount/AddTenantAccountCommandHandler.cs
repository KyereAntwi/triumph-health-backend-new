namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;

public sealed class AddTenantAccountCommandHandler(
    ITenantManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService,
    IPublishEndpoint publishEndpoint,
    ILogger<AddTenantAccountCommandHandler> logger) 
    : ICommandHandler<AddTenantAccountCommand, AddTenantAccountResponse>
{
    public async Task<BaseResponse<AddTenantAccountResponse>> HandleAsync(AddTenantAccountCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddTenantAccountCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<AddTenantAccountResponse>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Error",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        
        var existingTenantForUser = await dbContext
            .Tenants
            .FirstOrDefaultAsync(t => t.CreatedBy == loggedInUserService.UserId!, cancellationToken);

        if (existingTenantForUser is not null)
        {
            return new BaseResponse<AddTenantAccountResponse>
            {
                IsSuccess = false,
                Message = "Conflict",
                Errors = ["User already has a tenant account"],
                Status = 409
            };
        }

        var existingSubscription = await dbContext.Subscriptions.FindAsync(Guid.Parse(command.SubscriptionId), cancellationToken);
        if (existingSubscription is null)
        {
            return new BaseResponse<AddTenantAccountResponse>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Subscription was not found"]
            };
        }

        var newTenant = new Tenant
        {
            UniqueIdentifier = GenerateTenantIdentifier()
        };

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
        newTenant.TenantSubscriptions.Add(newTenantSubscription);
        
        newTenant.TenantManagers.Add(new TenantManager
        {
            ApplicationUserId = Guid.Parse(loggedInUserService.UserId!)
        });

        await dbContext.Tenants.AddAsync(newTenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await PublishTenantAccountAddedEvent(newTenantSubscription.Id);

        return new BaseResponse<AddTenantAccountResponse>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Tenant account created successfully",
            Data = new AddTenantAccountResponse(newTenant.Id, newTenant.UniqueIdentifier)
        };
    }

    private static string GenerateTenantIdentifier()
    {
        //TODO - implement identifier generation logic
        // pattern XX-XXXX-XXXX
        return "";
    }

    private async Task PublishTenantAccountAddedEvent(Guid tenantSubId)
    {
        var @event = new TenantAccountAddedEvent
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
            logger.LogError(e, "Problem publishing TenantAccountAddedEvent. Payload = {Payload}", @event);
        }
    }
}