namespace Triumph.HealthMs.Core.Features.General.AddLabTest;

public sealed class AddLabTestCommandHandler(
    ICommonEntitiesDbContext dbContext,
    IPermissionService permissionService) 
    : ICommandHandler<AddLabTestCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddLabTestCommand command, CancellationToken cancellationToken = default)
    {
        if (! await permissionService.UserHasRequiredPermission(PermissionType.ManageHealthInternals, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You are not permitted to perform this operation"]
            };
        }

        var validation = new AddLabTestCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var labTestAlreadyExists = await dbContext.LabTests
            .AnyAsync(l => l.Name.ToLower().Contains(command.Name.ToLower()), cancellationToken);

        if (labTestAlreadyExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["This Lab Test already exists"]
            };
        }

        var labTest = new LabTest
        {
            Name = command.Name,
            Description = command.Description
        };
        
        await dbContext.LabTests.AddAsync(labTest, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Lab test added successfully",
            Data = labTest.Id
        };
    }
}