namespace Triumph.HealthMs.Core.Features.EmployeeManagement.UpdateEmployeeRole;

public sealed class UpdateEmployeeRoleCommandHandler(
    IEmployeeManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService,
    ILogger<UpdateEmployeeRoleCommandHandler> logger,
    IPublishEndpoint publishEndpoint) 
    : ICommandHandler<UpdateEmployeeRoleCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateEmployeeRoleCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new UpdateEmployeeRoleCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        }
        
        var employeeExists =
            await dbContext.Employees.AnyAsync(e => e.Id == command.EmployeeId, cancellationToken);

        if (!employeeExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Selected employee was not found"]
            };
        }

        var roleExists = await dbContext.Roles.AnyAsync(r => r.Id == Guid.Parse(command.RoleId), cancellationToken);

        if (!roleExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Selected role was not found"]
            };
        }

        var mostRecentRole = await dbContext
            .EmployeeRoles
            .AsTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Where(r => r.EmployeeId == command.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (mostRecentRole is not null)
        {
            mostRecentRole.EndedAt = DateTime.Parse(command.OldRoleEndedAt);
            dbContext.EmployeeRoles.Update(mostRecentRole);
        }

        var newEmRole = new EmployeeRole
        {
            Id = Guid.CreateVersion7(),
            EmployeeId = command.EmployeeId,
            RoleId = Guid.Parse(command.RoleId),
            StartedFrom = DateTime.Parse(command.StartsAt)
        };
        await dbContext.EmployeeRoles.AddAsync(newEmRole, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await PublishEmployeeRoleUpdatedEvent(newEmRole.Id, cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Role updated successfully",
            Data = newEmRole.Id.ToString()
        };
    }

    private async Task PublishEmployeeRoleUpdatedEvent(Guid employeeRoleId, CancellationToken cancellationToken)
    {
        var @event = new EmployeeRoleUpdatedEvent
        {
            UserId = loggedInUserService.UserId!,
            Action = "Updated Employee role",
            EntityName = nameof(EmployeeRole),
            EntityId = employeeRoleId
        };

        try
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "There was a problem publishing EmployeeRoleUpdatedEvent. Payload: {Payload}", @event);
        }
    }
}