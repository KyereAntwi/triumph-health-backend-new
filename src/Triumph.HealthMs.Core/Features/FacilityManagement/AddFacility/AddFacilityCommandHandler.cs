namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddFacility;

public sealed class AddFacilityCommandHandler(
    ILoggedInUserService loggedInUserService,
    IFacilityManagementDbContext context,
    IPublishEndpoint publishEndpoint,
    ILogger<AddFacilityCommandHandler> logger) 
    : ICommandHandler<AddFacilityCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddFacilityCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddFacilityCommandValidator();
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

        var nameAlreadyExist =
            await context.OrganizationalFacilities.AnyAsync(f => 
                    f.Name.ToLower() == command.Name.ToLower() &&
                f.TenantId == Guid.Parse(loggedInUserService.TenantId!),
                cancellationToken);

        if (nameAlreadyExist)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["There is already a facility with selected name under this Tenant"]
            };
        }

        var urlSuffixAlreadyExist =
            await context.OrganizationalFacilities.AnyAsync(f =>
                f.UrlSuffix.ToLower() == command.UrlSuffix, cancellationToken);

        if (urlSuffixAlreadyExist)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["The chosen url suffix is already in use"]
            };
        }

        var newFacility = new OrganizationalFacility
        {
            Id = Guid.CreateVersion7(),
            TenantId = Guid.Parse(loggedInUserService.TenantId!),
            UrlSuffix = command.UrlSuffix,
            Name = command.Name,
            Address = command.Address,
            Email = command.Email,
            MainTelephone = command.MainTelephone,
            Description = command.Description,
            EstablishedAt = command.EstablishedAt != null ? DateOnly.Parse(command.EstablishedAt) : new DateOnly()
        };
        await context.OrganizationalFacilities.AddAsync(newFacility, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        // TODO - Add first manager and employee with transaction

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Facility added successfully",
            Data = newFacility.Id
        };
    }
    
    private async Task PublishFacilityAddedEvent(Guid facilityId)
    {
        var @event = new FacilityAddedEvent(facilityId)
        {
            UserId = loggedInUserService.UserId!,
            Action = "Facility Created",
            EntityName = nameof(OrganizationalFacility),
            EntityId = facilityId
        };

        try
        {
            await publishEndpoint.Publish(@event);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Problem publishing FacilityAddedEvent. Payload = {Payload}", @event);
        }
    }
}