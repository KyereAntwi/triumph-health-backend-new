namespace Triumph.HealthMs.Core.Features.ApplicationUser.UpdateUserInfomation;

public sealed class UpdateUserInformationCommandHandler(
    ILoggedInUserService loggedInUserService,
    IApplicationUserManagementDbContext dbContext) 
    : ICommandHandler<UpdateUserInformationCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateUserInformationCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new UpdateUserInformationCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        
        var existingAccount = await dbContext
            .ApplicationUsers
            .AsTracking()
            .FirstOrDefaultAsync(u => u.UserId == loggedInUserService.UserId, cancellationToken);

        if (existingAccount == null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["User account was not found"]
            };
        }
        
        existingAccount.FirstName = command.FirstName;
        existingAccount.LastName = command.LastName;
        existingAccount.OtherNames = command.OtherNames;
        existingAccount.Email = command.Email;
        existingAccount.PhoneNumber = command.PhoneNumber;
        existingAccount.Gender = Enum.Parse<Gender>(command.Gender);
        existingAccount.Nationality = Enum.Parse<Nationality>(command.Nationality);
        existingAccount.DateOfBirth = DateOnly.Parse(command.DateOfBirth);
        existingAccount.Title = command.Title;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "User Information Updated",
        };
    }
}