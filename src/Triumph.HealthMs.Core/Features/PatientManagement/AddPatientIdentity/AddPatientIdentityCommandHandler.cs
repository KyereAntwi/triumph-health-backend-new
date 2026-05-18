namespace Triumph.HealthMs.Core.Features.PatientManagement.AddPatientIdentity;

public sealed class AddPatientIdentityCommandHandler(
    IPermissionService permissionService,
    IPatientManagementDbContext dbContext) 
    : ICommandHandler<AddPatientIdentityCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddPatientIdentityCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.MANAGE_PATIENT_BIOGRAPHY, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You don't have permission to perform this operation"]
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

        var identification = new Identification
        {
            Id = Guid.CreateVersion7(),
            PatientId = command.PatientId,
            Type = Enum.Parse<IdentificationType>(command.Type),
            IdentificationNumber = command.IdentificationNumber,
            DateIssued = DateOnly.Parse(command.DateIssued),
            DateExpires = DateOnly.Parse(command.DateExpires),
            PlaceOfIssue = command.PlaceOfIssue,
            CountryOfIssue = command.CountryOfIssue
        };

        await dbContext.Identifications.AddAsync(identification, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Identification added successfully",
            Data = identification.Id
        };
    }
}