namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddAUiStorageItem;

public sealed class AddAUiStorageItemCommandHandler(
    IApplicationUserManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService) 
    : ICommandHandler<AddAUiStorageItemCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddAUiStorageItemCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddAUiStorageItemCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var keyExisting = await dbContext.UiStorageItems
            .AnyAsync(x => x.CreatedBy == loggedInUserService.UserId &&  x.Key == command.Key, cancellationToken);
        
        if(keyExisting)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 409,
                Message = "Conflict",
                Errors = ["A storage item with the same key already exists."]
            };
        }

        var item = new UiStorageItem
        {
            Key = command.Key,
            Value = command.Value,
        };
        
        await dbContext.UiStorageItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Status = 201,
            Message = "Storage item added successfully.",
            Data = item.Id
        };
    }
}