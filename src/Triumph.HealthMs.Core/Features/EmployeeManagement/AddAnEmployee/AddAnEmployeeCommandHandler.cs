namespace Triumph.HealthMs.Core.Features.EmployeeManagement.AddAnEmployee;

public sealed class AddAnEmployeeCommandHandler(
    IEmployeeManagementDbContext dbContext,
    IFacilityManagementDbContext facilityManagementDbContext,
    ILoggedInUserService loggedInUserService,
    IUpsetEmployeeService upsetEmployeeService,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<AddAnEmployeeCommandHandler> logger)
    : ICommandHandler<AddAnEmployeeCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddAnEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddAnEmployeeCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        }

        var isEmployeeExisting = await applicationUserManagementDbContext
            .ApplicationUsers
            .AnyAsync(e => e.Email == command.Email || e.PhoneNumber == command.PhoneNumber,
            cancellationToken);

        if (isEmployeeExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["Employee with this email or phone number already exists"]
            };
        }

        var isRoleExisting = await dbContext.Roles.AnyAsync(r => r.Id == Guid.Parse(command.RoleId), cancellationToken);
        if (!isRoleExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Selected role was not found"]
            };
        }

        var isFacilityExisting =
            await facilityManagementDbContext.OrganizationalFacilities.AnyAsync(
                f => f.Id == Guid.Parse(command.FacilityId), cancellationToken);
        if (!isFacilityExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Not Found",
                Errors = ["Selected facility was not found"]
            };
        }
        
        var isDepartmentExisting = await dbContext.Departments.AnyAsync(d => d.Id == Guid.Parse(command.DepartmentId), cancellationToken);
        if (!isDepartmentExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Not Found",
                Errors = ["Selected department was not found"]
            };
        }

        var (error, employeeId, linkId) = await upsetEmployeeService.UpsetEmployeeDetailsAsync(command, cancellationToken);

        if (!string.IsNullOrEmpty(error))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 500,
                Message = "Internal Server Error",
                Errors = [error]
            };
        }

        await PublishEmployeeAddedEvent((Guid)linkId!, cancellationToken);
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Employee added successfully",
            Data = (Guid)employeeId!
        };
    }

    private async Task PublishEmployeeAddedEvent(Guid invitationId, CancellationToken cancellationToken)
    {
        var @event = new EmployeeAddedEvent
        {
            EntityName = nameof(LinkInvitation),
            EntityId = invitationId,
            UserId = loggedInUserService.UserId!,
            Action = "Employee created"
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
