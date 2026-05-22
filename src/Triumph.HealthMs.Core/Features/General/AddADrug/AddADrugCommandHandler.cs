namespace Triumph.HealthMs.Core.Features.General.AddADrug;

public sealed class AddADrugCommandHandler(
    ICommonEntitiesDbContext dbContext,
    IPermissionService permissionService) 
    : ICommandHandler<AddADrugCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddADrugCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.ManageHealthInternals, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You do not have permission to perform this operation"]
            };
        }
        
        var validation = new AddADrugCommandValidator();
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

        var drugAlreadyExists =
            await dbContext.Drugs.AnyAsync(d => d.Name.ToLower().Contains(command.Name.ToLower()), cancellationToken);

        if (drugAlreadyExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["This drug already exists"]
            };
        }

        var drug = new Drug
        {
            Name = command.Name,
            Description = command.Description,
            Prescription = command.Prescription,
            Manufacturer = command.Manufacturer
        };
        
        await dbContext.Drugs.AddAsync(drug, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Drug added successfully",
            Data = drug.Id
        };
    }
}