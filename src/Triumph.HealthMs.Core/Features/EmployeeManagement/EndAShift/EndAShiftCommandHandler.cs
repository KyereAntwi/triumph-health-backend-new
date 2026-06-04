namespace Triumph.HealthMs.Core.Features.EmployeeManagement.EndAShift;

public sealed class EndAShiftCommandHandler(
    IEmployeeManagementDbContext dbContext, 
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext appUserDbContext) 
    : ICommandHandler<EndAShiftCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(EndAShiftCommand command, CancellationToken cancellationToken = default)
    {
        var userId = await appUserDbContext
            .ApplicationUsers
            .Where(a => a.UserId == loggedInUserService.UserId)
            .Select(a => a.Id)
            .FirstAsync(cancellationToken);

        var employeeId = await dbContext.Employees
            .Where(e => e.ApplicationUserId == userId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (employeeId == Guid.Empty)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not found",
                Errors = ["Employee not found"]
            };
        }
        
        var availableShift = await dbContext.EmployeeShifts
            .Where(s => s.EmployeeId == employeeId && s.TimeStamp.Date == DateTime.UtcNow.Date)
            .Select(s => new { s.Id, s.StartedAt, s.EndedAt })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (availableShift is null)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not found",
                Errors = ["No available shift"]
            };
        }

        if (availableShift.StartedAt is null || availableShift.EndedAt is not null)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["Shift might not be started or might already be closed"]
            };
        }
        
        await dbContext.EmployeeShifts
            .Where(s => s.Id == availableShift.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.EndedAt, DateTime.UtcNow), cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Shift ended successfully",
            Data = availableShift.Id
        };
    }
}