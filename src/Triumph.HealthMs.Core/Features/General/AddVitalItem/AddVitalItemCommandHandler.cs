namespace Triumph.HealthMs.Core.Features.General.AddVitalItem;

public sealed class AddVitalItemCommandHandler(
    IPermissionService permissionService,
    ICommonEntitiesDbContext dbContext) 
    : ICommandHandler<AddVitalItemCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddVitalItemCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.ManagePatientVitals, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You are not allowed to perform this operation"]
            };
        }

        var validation = new AddVitalItemCommandValidator();
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

        var vitalAlreadyExists =
            await dbContext.VitalItems.AnyAsync(v => v.Name.ToLower().Contains(command.Name.ToLower()),
                cancellationToken);

        if (vitalAlreadyExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["This vital item already exists"]
            };
        }


        var vitalItem = new VitalItem
        {
            Name = command.Name,
            Description = command.Description
        }; 
        
        await dbContext.VitalItems.AddAsync(vitalItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Vital Item saved successfully",
            Data = vitalItem.Id
        };
    }
}