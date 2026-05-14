namespace Triumph.HealthMs.Core.Features.PatientManagment.AddPatient;

public sealed class AddPatientCommandHandler(
    IPermissionService permissionService,
    IPatientUpsetService upsetService,
    ILogger<AddPatientCommandHandler> logger,
    IPublishEndpoint publishEndpoint,
    ILoggedInUserService loggedInUserService) 
    : ICommandHandler<AddPatientCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddPatientCommand command, CancellationToken cancellationToken = default)
    {
        if (! await permissionService.UserHasRequiredPermission(PermissionType.MANAGE_PATIENT_BIOGRAPHY, cancellationToken))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 403,
                Message = "Forbidden",
                Errors = ["You dont permission to perform this operation"]
            };
        }

        var validation = new AddPatientCommandValidator();
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

        var (error, id) = await upsetService.UpsetPatientDetails(command, cancellationToken);

        if (!string.IsNullOrEmpty(error))
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 500,
                Message = "Internal Server Error",
                Errors = ["There was a problem completing adding patient details"]
            };
        }

        if (command.SendAccountLinkageInvitation)
        {
            await PublishPatientAddedEvent((Guid)id!, cancellationToken);
        }

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Patient added successfully",
            Data = (Guid)id!
        };
    }

    private async Task PublishPatientAddedEvent(Guid patientId, CancellationToken cancellationToken)
    {
        var @event = new PatientAddedEvent
        {
            UserId = loggedInUserService.UserId!,
            Action = "Added a Patient",
            EntityName = nameof(Patient),
            EntityId = patientId
        };

        try
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error publishing PatientAddedEvent. Payload {Payload}", @event);
        }
    }
}