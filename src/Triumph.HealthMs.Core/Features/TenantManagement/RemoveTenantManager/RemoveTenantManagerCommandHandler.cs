namespace Triumph.HealthMs.Core.Features.TenantManagement.RemoveTenantManager;

public sealed class RemoveTenantManagerCommandHandler(
    ILoggedInUserService loggedInUserService,
    ITenantManagementDbContext dbContext,
    IApplicationUserManagementDbContext applicationUserDbContext) 
    : ICommandHandler<RemoveTenantManagerCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(RemoveTenantManagerCommand command, CancellationToken cancellationToken = default)
    {
        var existingUser = await applicationUserDbContext
            .ApplicationUsers
            .Select(a => new
            {
                a.UserId,
                a.Id
            })
            .FirstOrDefaultAsync(a => a.UserId == loggedInUserService.UserId, cancellationToken);
        
        var isAManager = await dbContext
            .TenantManagers
            .AnyAsync(m => m.ApplicationUserId == existingUser!.Id && m.TenantId == Guid.Parse(loggedInUserService.TenantId!), cancellationToken);
        
        if (!isAManager)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You are not a manager."]
            };
        }

        var existingManager = await dbContext.TenantManagers.FindAsync(command.TenantManagerId, cancellationToken);

        if (existingManager is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Tenant manager was not found."]
            };
        }

        dbContext.TenantManagers.Remove(existingManager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200
        };
    }
}