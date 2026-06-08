namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetUiStorageItemsQueryHandler(
    IApplicationUserManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService)
    : IQueryHandler<object, IEnumerable<UiStorageItemDto>>
{
    public async Task<BaseResponse<IEnumerable<UiStorageItemDto>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.UiStorageItems
            .Where(x => x.CreatedBy == loggedInUserService.UserId)
            .Select(x => new UiStorageItemDto(x.Key, x.Value))
            .ToArrayAsync(cancellationToken);
        
        return new BaseResponse<IEnumerable<UiStorageItemDto>>
        {
            IsSuccess = true,
            Data = items
        };
    }
}