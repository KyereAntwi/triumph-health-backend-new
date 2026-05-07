namespace Triumph.HealthMs.Core.Features.ApplicationUser.LinkUserToExistingAccount;

public sealed class LinkUserToExistingAccountCommandHandler(
    ITenantManagementDbContext dbContext,
    ILogger<LinkUserToExistingAccountCommandHandler> logger,
    ILoggedInUserService loggedInUserService,
    IPublishEndpoint publishEndpoint) 
    : ICommandHandler<LinkUserToExistingAccountCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(LinkUserToExistingAccountCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new LinkUserToExistingAccountCommandValidator();
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
        
        var existingInvitationLink = await dbContext
            .LinkInvitations
            .FirstOrDefaultAsync(li => li.Id == Guid.Parse(command.LinkId), cancellationToken);

        if (existingInvitationLink is null)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Message = "Not Found",
                Errors = ["Invitation not found"],
                Status = 404
            };
        }
        
        if (existingInvitationLink.ExpiresAt < DateTime.UtcNow || existingInvitationLink.Linked)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Message = "Link Expired",
                Errors = ["The invitation link has expired or might be used"],
                Status = 409
            };
        }
        
        existingInvitationLink.ApplicationUser!.UserId = loggedInUserService.UserId!;

        await using (var transaction = await ((DbContext)dbContext).Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                await dbContext.ApplicationUsers
                    .Where(au => au.Id == existingInvitationLink.ApplicationUserId)
                    .ExecuteUpdateAsync(
                        u => u.SetProperty(a => a.UserId, loggedInUserService.UserId!),
                        cancellationToken);
                
                await dbContext.LinkInvitations
                    .Where(li => li.Id == existingInvitationLink.Id)
                    .ExecuteUpdateAsync(
                        u => u.SetProperty(li => li.Linked, true),
                        cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"An error occurred while linking user {UserId} to existing account for LinkId: {LinkId}", loggedInUserService.UserId, command.LinkId);
                await transaction.RollbackAsync(cancellationToken);
                return new BaseResponse<Guid>
                {
                    IsSuccess = false,
                    Message = "Server Error",
                    Errors = ["An error occurred while linking the user to the existing account."],
                    Status = 500
                };
            }
        }
        
        await PublishUserAccountLinkedEvent(existingInvitationLink.Id, cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Message = "User linked successfully",
            Data = existingInvitationLink.ApplicationUserId,
            Status = 200
        };
    }

    private async Task PublishUserAccountLinkedEvent(Guid linkId, CancellationToken cancellationToken)
    {
        var @event = new UserAccountAddedEvent
        {
            UserId = loggedInUserService.UserId!,
            EntityName = "LinkInvitation",
            EntityId = linkId,
            Action = "Account linked to User",
        };

        try
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to publish UserAccountAddedEvent with payload {Payload}", @event);
        }
    }
}