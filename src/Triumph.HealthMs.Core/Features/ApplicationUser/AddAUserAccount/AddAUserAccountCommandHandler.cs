namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddAUserAccount;

public sealed class AddAUserAccountCommandHandler(
    ILoggedInUserService loggedInUserService,
    ITenantManagementDbContext dbContext,
    IPublishEndpoint  publishEndpoint,
    ILogger<AddAUserAccountCommandHandler> logger) 
    : ICommandHandler<AddAUserAccountCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddAUserAccountCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddAUserAccountCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var newUser = new Models.ApplicationUser.ApplicationUser
        {
            UserId = loggedInUserService.UserId!,
            FirstName = command.FirstName,
            LastName = command.LastName,
            OtherNames = command.OtherNames,
            Gender = Enum.Parse<Gender>(command.Gender),
            Nationality = Enum.Parse<Nationality>(command.Nationality),
            DateOfBirth = DateOnly.Parse(command.DateOfBirth),
            Email = command.Email,
            PhoneNumber = command.PhoneNumber
        };
        
        await dbContext.ApplicationUsers.AddAsync(newUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        await PublishEventAsync(command, newUser.Id, cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "User account created successfully",
            Data = newUser.Id
        };
    }

    private async Task PublishEventAsync(AddAUserAccountCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var @event = new UserAccountAddedEvent
        {
            Email = command.Email ??  string.Empty,
            PhoneNumber = command.PhoneNumber,
            FullName = $"{command.FirstName} {command.OtherNames} {command.LastName}".Trim(),
            UserId = loggedInUserService.UserId!,
            Action = "Create user account",
            EntityName = "ApplicationUser",
            EntityId = userId
        };
        
        try
        {
            await publishEndpoint.Publish(@event,  cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to publish UserAccountAddedEvent with event payload {Payload}", @event);
        }
    }
}