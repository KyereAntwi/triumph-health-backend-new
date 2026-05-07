namespace Triumph.HealthMs.Core.Features.TenantManagement.AddTenantManager;

public sealed class AddTenantManagerCommandHandler(
    ILoggedInUserService loggedInUserService,
    ITenantManagementDbContext dbContext,
    IApplicationUserManagementDbContext applicationUserDbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<AddTenantManagerCommandHandler> logger) 
    : ICommandHandler<AddTenantManagerCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddTenantManagerCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddTenantManagerCommandValidator();
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

        var tenantManagers = await dbContext
            .TenantManagers
            .Where(t => t.TenantId == Guid.Parse(loggedInUserService.TenantId!))
            .Select(t => t.ApplicationUserId)
            .ToArrayAsync(cancellationToken);

        if (tenantManagers.Contains(Guid.Parse(command.ApplicationUserId)))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["User is already a manager."]
            };
        }

        var newManager = new TenantManager
        {
            Id = Guid.CreateVersion7(),
            TenantId = Guid.Parse(loggedInUserService.TenantId!),
            ApplicationUserId = Guid.Parse(command.ApplicationUserId)
        };
        await dbContext.TenantManagers.AddAsync(newManager, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await PublishTenantManagerAddedEvent(newManager.Id);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Manager added successfully",
            Data = newManager.Id
        };
    }
    
    private async Task PublishTenantManagerAddedEvent(Guid tenantManagerId)
    {
        var @event = new TenantManagerAddedEvent(tenantManagerId)
        {
            UserId = loggedInUserService.UserId!,
            Action = "Added Tenant Manager",
            EntityName = nameof(TenantManager),
            EntityId = tenantManagerId
        };

        try
        {
            await publishEndpoint.Publish(@event);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Problem publishing PublishTenantManagerAddedEvent. Payload = {Payload}", @event);
        }
    }
}