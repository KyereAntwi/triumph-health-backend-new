namespace Triumph.HealthMs.Core.Features.EmployeeManagement.StartAShift;

public sealed class StartAShiftCommandHandler(
    IEmployeeManagementDbContext dbContext,
    IApplicationUserManagementDbContext appUserDbContext,
    ILoggedInUserService loggedInUserService)
    : ICommandHandler<StartAShiftCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(StartAShiftCommand command, CancellationToken cancellationToken = default)
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
            .Select(s => new
            {
                s.Id, 
                s.StartedAt
            })
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

        if (availableShift.StartedAt is not null)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["Shift already started"]
            };
        }
        
        await dbContext.EmployeeShifts
            .Where(s => s.Id == availableShift.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.StartedAt, DateTime.UtcNow), cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Shift started successfully",
            Data = availableShift.Id
        };
    }
}