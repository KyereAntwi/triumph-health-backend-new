namespace Triumph.HealthMs.Core.Features.EmployeeManagement.AddEmployeeShift;

public sealed class AddEmployeeShiftCommandHandler(
    IEmployeeManagementDbContext dbContext) 
    : ICommandHandler<AddEmployeeShiftCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddEmployeeShiftCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddEmployeeShiftCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        
        var isEmployeeExisting = await dbContext.Employees.AnyAsync(e => e.Id == command.EmployeeId, cancellationToken);
        if (!isEmployeeExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not found.",
                Errors = ["Employee was not found."]
            };
        }

        if (command.ArchivePreviouslyActiveOnes)
        {
            // mark old and already attended shifts as deleted/archived
            // they would not be fetched when employee shifts are fetched normally unless archived shifts are explicitly fetched
            await dbContext.EmployeeShifts
                .Where(es => es.EmployeeId == command.EmployeeId 
                             && !es.Deleted 
                             && es.StartedAt != null 
                             && es.EndedAt != null)
                .ExecuteUpdateAsync(s => s.SetProperty(es => es.Deleted, true), cancellationToken);
        }
        
        await dbContext.EmployeeShifts.AddRangeAsync(GenerateShifts(command), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Employee shifts added successfully.",
            Data = command.EmployeeId
        };
    }
    
    private static List<EmployeeShift> GenerateShifts(AddEmployeeShiftCommand command)
    {
        List<EmployeeShift> shifts = [];
        var dateToStopAt = DateTime.Parse(command.ShiftStartsAt);

        if (command.Recurring)
        {
            while (dateToStopAt <= DateTime.Parse(command.RecurringUntil))
            {
                shifts.Add(new EmployeeShift
                {
                    EmployeeId = command.EmployeeId,
                    ShiftDurationInHours = command.ShiftDurationInHours,
                    ShiftType = Enum.Parse<ShiftType>(command.ShiftType),
                    DayOfWeek = command.DayOfWeek,
                    TimeStamp = dateToStopAt
                });
                
                dateToStopAt = dateToStopAt.AddDays(7);
            }
        }
        else
        {
            shifts.Add(new EmployeeShift
            {
                EmployeeId = command.EmployeeId,
                ShiftDurationInHours = command.ShiftDurationInHours,
                ShiftType = Enum.Parse<ShiftType>(command.ShiftType),
                DayOfWeek = command.DayOfWeek,
                TimeStamp = dateToStopAt
            });
        }
        
        return shifts;
    }
}