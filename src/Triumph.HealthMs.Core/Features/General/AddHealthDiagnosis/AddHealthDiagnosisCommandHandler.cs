namespace Triumph.HealthMs.Core.Features.General.AddHealthDiagnosis;

public sealed class AddHealthDiagnosisCommandHandler(
    ICommonEntitiesDbContext dbContext,
    IPermissionService permissionService) 
    : ICommandHandler<AddHealthDiagnosisCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddHealthDiagnosisCommand command, CancellationToken cancellationToken = default)
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
        
        var validation = new AddHealthDiagnosisCommandValidator();
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

        var diagnosisAlreadyExists = await dbContext.HealthDiagnoses
            .AnyAsync(d => d.Name.ToLower().Contains(command.Name.ToLower()), cancellationToken);

        if (diagnosisAlreadyExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["This Health Diagnosis already exists"]
            };
        }

        var healthDiagnosis = await dbContext.HealthDiagnoses.AddAsync(new HealthDiagnosis
        {
            Name = command.Name,
            Description = command.Description,
            RecommendedPrescription = command.RecommendedPrescription
        }, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Health Diagnosis added successfully",
            Data = healthDiagnosis.Entity.Id
        };
    }
}