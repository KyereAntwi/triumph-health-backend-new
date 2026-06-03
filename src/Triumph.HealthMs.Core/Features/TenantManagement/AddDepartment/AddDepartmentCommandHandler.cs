namespace Triumph.HealthMs.Core.Features.TenantManagement.AddDepartment;

public sealed class AddDepartmentCommandHandler(
    IEmployeeManagementDbContext dbContext) 
    : ICommandHandler<AddDepartmentCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddDepartmentCommandValidator();
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

        var alreadyExists = await dbContext.Departments
            .AnyAsync(d => d.Name.ToLower() == command.Name.ToLower(), cancellationToken);

        if (alreadyExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["This name is already used for another department"]
            };
        }

        var department = new Department
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            Description = command.Description
        };
        
        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Department added successfully",
            Data = department.Id
        };
    }
}