namespace Triumph.HealthMs.Core.Features.FacilityManagement.RemoveManager;

public sealed class RemoveFacilityManagerCommandHandler(
    IFacilityManagementDbContext dbContext) 
    : ICommandHandler<RemoveFacilityManagerCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(RemoveFacilityManagerCommand command, CancellationToken cancellationToken = default)
    {
        var facilityManager = await dbContext.FacilityManagers.FindAsync(command.ManagerId, cancellationToken);

        if (facilityManager is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Manager was not found"]
            };
        }

        dbContext.FacilityManagers.Remove(facilityManager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200
        };
    }
}