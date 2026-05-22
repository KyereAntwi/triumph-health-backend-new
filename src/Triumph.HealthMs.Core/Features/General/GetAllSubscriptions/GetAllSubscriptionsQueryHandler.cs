namespace Triumph.HealthMs.Core.Features.General.GetAllSubscriptions;

public sealed class GetAllSubscriptionsQueryHandler 
    (ITenantManagementDbContext dbContext): IQueryHandler<GetAllSubscriptionsQuery, IEnumerable<SubscriptionDto>>
{
    public async Task<BaseResponse<IEnumerable<SubscriptionDto>>> HandleAsync(GetAllSubscriptionsQuery query, CancellationToken cancellationToken = default)
    {
        var list =  await dbContext
            .Subscriptions
            .Select(s => new SubscriptionDto(
                Id: s.Id.ToString(),
                Title: s.Title,
                Description: s.Description,
                CostPerMonth: s.CostPerMonth))
            .ToArrayAsync<SubscriptionDto>(cancellationToken);

        return new BaseResponse<IEnumerable<SubscriptionDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}