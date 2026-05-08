namespace Triumph.HealthMs.Core.Features.FacilityManagement.UpdateFacility;

public sealed class UpdateFacilityCommandHandler (
    IFacilityManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService)
    : ICommandHandler<UpdateFacilityCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateFacilityCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new UpdateFacilityCommandValidator();
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

        var existingFacility = await dbContext
            .OrganizationalFacilities
            .AsTracking()
            .Where(f => f.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingFacility is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Facility was not found"]
            };
        }

        if (!existingFacility.Name.Equals(command.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            var nameAlreadyExist =
                await dbContext.OrganizationalFacilities.AnyAsync(f => 
                        f.Name.ToLower() == command.Name.ToLower() &&
                        f.TenantId == Guid.Parse(loggedInUserService.TenantId!),
                    cancellationToken);

            if (nameAlreadyExist)
            {
                return new BaseResponse<string>
                {
                    IsSuccess = false,
                    Status = 409,
                    Message = "Conflict",
                    Errors = ["There is already a facility with selected name under this Tenant"]
                };
            }
            
            existingFacility.Name = command.Name;
        }
        
        existingFacility.Address = command.Address;
        existingFacility.Email = command.Email;
        existingFacility.MainTelephone = command.MainTelePhone;
        existingFacility.Description = command.Description;
        existingFacility.EstablishedAt = command.EstablishedAt != null ? DateOnly.Parse(command.EstablishedAt) : null;
        
        dbContext.OrganizationalFacilities.Update(existingFacility);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Facility updated successfully"
        };
    }
}