namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;

public sealed class AddTenantAccountCommandHandler(
    ITenantManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService,
    IPublishEndpoint publishEndpoint,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
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
            Id = Guid.CreateVersion7(),
            UniqueIdentifier = GenerateTenantIdentifier(),
            OrganizationTitle = command.OrganizationalTitle,
            Email = command.Email,
            Address = command.Address,
            MainTelephone = command.MainTelephone
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

        var appUserId = await applicationUserManagementDbContext
            .ApplicationUsers
            .Where(a => a.UserId == loggedInUserService.UserId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        newTenant.TenantManagers.Add(new TenantManager
        {
            ApplicationUserId = appUserId,
            TenantId = newTenant.Id
        });

        await dbContext.Tenants.AddAsync(newTenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await PublishTenantAccountAddedEvent(newTenantSubscription.Id, newTenant.Id);

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
        var now = DateTime.UtcNow;
        var firstLetter = (char)Random.Shared.Next('A', 'Z' + 1);
        var secondLetter = (char)Random.Shared.Next('A', 'Z' + 1);

        return $"{firstLetter}{secondLetter}-{now:yyMM}-{now:mmss}";
    }

    private async Task PublishTenantAccountAddedEvent(Guid tenantSubId, Guid tenantId)
    {
        var @event = new TenantAccountAddedEvent
        {
            TenantSubscription = tenantSubId,
            UserId = loggedInUserService.UserId!,
            Action = "Tenant Account Created",
            EntityName = nameof(Tenant),
            EntityId = tenantId
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