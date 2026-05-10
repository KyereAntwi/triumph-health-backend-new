namespace Triumph.HealthMs.Core.Features.EmployeeManagement.UpdateEmployeePermissions;

public sealed class UpdateEmployeePermissionsCommandHandler(
    IEmployeeManagementDbContext dbContext,
    IUpsetEmployeeService upsetEmployeeService) 
    : ICommandHandler<UpdateEmployeePermissionsCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateEmployeePermissionsCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new UpdateEmployeePermissionsCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        }

        var existingEmployee =
            await dbContext
                .Employees
                .Include(e => e.EmployeePermissions)
                .Where(e => e.Id == command.EmployeeId)
                .FirstOrDefaultAsync(cancellationToken);

        if (existingEmployee is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Selected employee was not found"]
            };
        }

        var result =
            await upsetEmployeeService.UpdateEmployeePermissionsAsync(existingEmployee, command, cancellationToken);

        if (!string.IsNullOrEmpty(result))
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 500,
                Message = "Internal Server Error",
                Errors = ["There was a problem updating employee permissions"]
            };
        }

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Permissions updated successfully"
        };
    }
}