namespace Triumph.HealthMs.Core.Features.PatientManagement.TakeVitalMeasurement;

public sealed class TakeVitalMeasurementCommandHandler (
    ILoggedInUserService loggedInUserService,
    IPermissionService permissionService,
    IPatientManagementDbContext dbContext,
    ICommonEntitiesDbContext commonEntitiesDbContext)
    : ICommandHandler<TakeVitalMeasurementCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(TakeVitalMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasRequiredPermission(PermissionType.ManagePatientVitals, cancellationToken))
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You do not have permission to perform this operation"]
            };
        }
        
        var validation = new TakeVitalMeasurementCommandValidator();
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

        var facilityId = Guid.Parse(loggedInUserService.FacilityId!);
        
        var patientExists = await dbContext
            .Patients.AnyAsync(p => p.Id == command.PatientId && p.FacilityId == facilityId, cancellationToken);
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
        
        var visitationExists = await dbContext.Visitations.AnyAsync(v => v.Id == command.VisitationId && v.FacilityId == facilityId, cancellationToken);
        if (!visitationExists)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Visitation was not found"]
            };
        }

        var vitalItemIds = command.VitalMeasurements.Select(vm => Guid.Parse(vm.VitalItemId)).ToList();
        var existingVitalItemIds = await commonEntitiesDbContext.VitalItems
            .Where(v => vitalItemIds.Contains(v.Id))
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);

        var nonExistentVitalItemIds = vitalItemIds.Except(existingVitalItemIds).ToArray();

        if (nonExistentVitalItemIds.Length != 0)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = nonExistentVitalItemIds.Select(id => $"Vital item with ID '{id}' was not found").ToArray()
            };
        }

        await dbContext.PatientVitals.AddRangeAsync(command.VitalMeasurements.Select(v => new PatientVital
        {
            Id = Guid.CreateVersion7(),
            VitalItemId = Guid.Parse(v.VitalItemId),
            VisitationId = command.VisitationId,
            MeasurementValue = v.MeasurementValue,
            Notes = v.Notes
        }), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Success"
        };
    }
}