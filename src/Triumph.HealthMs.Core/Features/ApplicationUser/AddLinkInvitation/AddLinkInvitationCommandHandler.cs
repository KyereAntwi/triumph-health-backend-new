namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddLinkInvitation;

public sealed class AddLinkInvitationCommandHandler(
    IApplicationUserManagementDbContext userDbContext,
    IEmployeeManagementDbContext employeeDbContext,
    IPatientManagementDbContext patientDbContext,
    ILogger<AddLinkInvitationCommandHandler> logger,
    IPublishEndpoint publishEndpoint,
    ILoggedInUserService loggedInUserService) 
    : ICommandHandler<AddLinkInvitationCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddLinkInvitationCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddLinkInvitationCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Message = "Validation Failed",
                Status = 400,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var appUserId = Guid.Empty;
        
        if(string.Equals(command.EntityType, "Employee", StringComparison.OrdinalIgnoreCase))
        {
            appUserId = await employeeDbContext
                .Employees
                .Where(e => e.Id == Guid.Parse(command.EntityId))
                .Select(e => e.ApplicationUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (appUserId == Guid.Empty)
            {
                return new BaseResponse<Guid>
                {
                    IsSuccess = false,
                    Status = 404,
                    Message = "Not Found",
                    Errors = ["Employee not found"]
                };
            }
        }
        if (string.Equals(command.EntityType, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            appUserId = await patientDbContext
                .Patients
                .Where(p => p.Id == Guid.Parse(command.EntityId))
                .Select(p => p.ApplicationUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (appUserId == Guid.Empty)
            {
                return new BaseResponse<Guid>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    Status = 404,
                    Errors = ["Patient not found"]
                };
            }
        }

        await userDbContext
            .LinkInvitations
            .Where(l => l.ApplicationUserId == appUserId)
            .ExecuteUpdateAsync(li => li.SetProperty(l => l.ExpiresAt, DateTime.UtcNow), cancellationToken);

        var invitation = new LinkInvitation
        {
            Id = Guid.CreateVersion7(),
            InvitedEntityType = command.EntityType,
            ApplicationUserId = appUserId
        };
        
        await userDbContext.LinkInvitations.AddAsync(invitation, cancellationToken);
        await userDbContext.SaveChangesAsync(cancellationToken);

        await PublishInvitationAddedEvent(invitation.Id, command.EntityType, cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Message = "Link invitation added successfully",
            Status = 201,
            Data = invitation.Id
        };
    }

    private async Task PublishInvitationAddedEvent(Guid invitationId, string invitedEntityType, CancellationToken cancellationToken)
    {
        var @event = new InvitationAddedEvent(invitedEntityType)
        {
            EntityName = nameof(LinkInvitation),
            EntityId = invitationId,
            UserId = loggedInUserService.UserId!,
            Action = "Invitation created"
        };

        try
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error publishing PublishEmployeeAddedEvent. Payload: {Payload}", @event);
        }
    }
}