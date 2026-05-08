namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddManager;

public sealed class AddFacilityManagerCommandHandler(
    ILoggedInUserService loggedInUserService,
    IFacilityManagementDbContext dbContext,
    IApplicationUserManagementDbContext userManagementDbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<AddFacilityManagerCommandHandler> logger) 
    : ICommandHandler<AddFacilityManagerCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(AddFacilityManagerCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddFacilityManagerCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Error",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var isAlreadyManager = await dbContext
            .FacilityManagers
            .AnyAsync(m => m.ApplicationUserId == Guid.Parse(command.ApplicationUserId),
                cancellationToken);

        if (isAlreadyManager)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["User is already a manager."]
            };
        }

        var userExists =
            await userManagementDbContext.ApplicationUsers.AnyAsync(a => a.Id == Guid.Parse(command.ApplicationUserId),
                cancellationToken);

        if (!userExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["User was not found."]
            };
        }

        var newManager = new FacilityManager
        {
            Id = Guid.CreateVersion7(),
            ApplicationUserId = Guid.Parse(command.ApplicationUserId),
            FacilityId = command.FacilityId,
            TenantId = Guid.Parse(loggedInUserService.TenantId!)
        };
        
        await dbContext.FacilityManagers.AddAsync(newManager, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await PublishFacilityManagerAddedEvent(newManager.Id);
        
        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Manager added successfully",
            Data = newManager.Id.ToString()
        };
    }
    
    private async Task PublishFacilityManagerAddedEvent(Guid facilityManagerId)
    {
        var @event = new FacilityManagerAddedEvent
        {
            UserId = loggedInUserService.UserId!,
            Action = "Added a Facility Manager",
            EntityName = nameof(FacilityManager),
            EntityId = facilityManagerId
        };

        try
        {
            await publishEndpoint.Publish(@event);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Problem publishing PublishFacilityManagerAddedEvent. Payload = {Payload}", @event);
        }
    }
}