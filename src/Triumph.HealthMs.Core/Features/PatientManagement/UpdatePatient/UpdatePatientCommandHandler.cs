namespace Triumph.HealthMs.Core.Features.PatientManagement.UpdatePatient;

public sealed class UpdatePatientCommandHandler(
    IPatientManagementDbContext dbContext,
    IPermissionService permissionService,
    IPatientUpsetService upsetService) 
    : ICommandHandler<UpdatePatientCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdatePatientCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.MANAGE_PATIENT_BIOGRAPHY,
                cancellationToken))
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You do not have permission to perform this operation"]
            };
        }

        var validation = new UpdatePatientCommandValidator();
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

        var patientExists = await dbContext.Patients.AnyAsync(p => p.Id == command.PatientId, cancellationToken);
        if (!patientExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Patient was not found"]
            };
        }

        var upsetError = await upsetService.UpsetForUpdatePatientDetails(command, cancellationToken);

        if (!string.IsNullOrEmpty(upsetError))
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 500,
                Message = "Internal Server Error",
                Errors = ["There was a problem completing the update"]
            };
        }

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Updates completed successfully"
        };
    }
}