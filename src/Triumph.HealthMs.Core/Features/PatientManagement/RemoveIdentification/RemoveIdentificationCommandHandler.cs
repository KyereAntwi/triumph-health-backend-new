namespace Triumph.HealthMs.Core.Features.PatientManagement.RemoveIdentification;

public sealed class RemoveIdentificationCommandHandler(
    IPermissionService permissionService,
    IPatientManagementDbContext dbContext) 
    : ICommandHandler<RemoveIdentificationCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(RemoveIdentificationCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.MANAGE_PATIENT_BIOGRAPHY, cancellationToken))
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You do not have permission to perform operation"]
            };
        }

        var patientExists = await dbContext.Patients.AnyAsync(p => p.Id == command.PatientId, cancellationToken);

        if (!patientExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Patient does not exist"]
            };
        }

        var identification = await dbContext
            .Identifications
            .Where(id => id.Id == command.IdentificationId && id.PatientId == command.PatientId)
            .FirstOrDefaultAsync(cancellationToken);

        if (identification is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Identification does not exist"]
            };
        }

        dbContext.Identifications.Remove(identification);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Operation was successful"
        };
    }
}