namespace Triumph.HealthMs.Core.Features.PatientManagement.AddVisit;

public sealed class AddVisitCommandHandler(
    IPermissionService permissionService,
    IPatientManagementDbContext dbContext) 
    : ICommandHandler<AddVisitCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddVisitCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.ManagePatientVisits, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You do not have permission to perform this operation"]
            };
        }
        
        var validation = new AddVisitCommandValidator();
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

        var patientExists = await dbContext.Patients.AnyAsync(p => p.Id == command.PatientId, cancellationToken);

        if (!patientExists)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Patient was not found"]
            };
        }

        var visitation = new Visitation
        {
            Id = Guid.CreateVersion7(),
            PatientId = command.PatientId,
            VisitingReason = command.VisitReason
        };
        
        await dbContext.Visitations.AddAsync(visitation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Created",
            Data = visitation.Id
        };
    }
}