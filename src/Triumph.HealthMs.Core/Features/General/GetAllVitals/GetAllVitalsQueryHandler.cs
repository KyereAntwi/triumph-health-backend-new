namespace Triumph.HealthMs.Core.Features.General.GetAllVitals;

public sealed class GetAllVitalsQueryHandler(
    ICommonEntitiesDbContext dbContext) 
    : IQueryHandler<GetAllVitalsQuery, IEnumerable<VitalItemDto>>
{
    public async Task<BaseResponse<IEnumerable<VitalItemDto>>> HandleAsync(GetAllVitalsQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext.VitalItems.AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            innerQuery = innerQuery.Where(v => v.Name.ToLower().Contains(query.SearchKey.ToLower()));
        }

        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        var pagedList = await innerQuery
            .OrderByDescending(v => v.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VitalItemDto(v.Id.ToString(), v.Name, v.Description))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<VitalItemDto>>
        {
            IsSuccess = true,
            Data = pagedList
        };
    }
}