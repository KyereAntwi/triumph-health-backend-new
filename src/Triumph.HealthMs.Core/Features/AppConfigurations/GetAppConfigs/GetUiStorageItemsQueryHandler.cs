namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetUiStorageItemsQueryHandler(
    IApplicationUserManagementDbContext dbContext)
    : IQueryHandler<object, IEnumerable<UiStorageItemDto>>
{
    public async Task<BaseResponse<IEnumerable<UiStorageItemDto>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;
        var items = await dbContext.UiStorageItems
            .Where(x => x.CreatedBy == ctx.UserId)
            .Select(x => new UiStorageItemDto(x.Key, x.Value))
            .ToArrayAsync(cancellationToken);
        
        return new BaseResponse<IEnumerable<UiStorageItemDto>>
        {
            IsSuccess = true,
            Data = items
        };
    }
}