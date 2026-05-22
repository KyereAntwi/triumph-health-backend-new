namespace Triumph.HealthMs.Core.Features.General.GetAllLabTests;

public sealed class GetAllLabTestsQueryHandler (
    ICommonEntitiesDbContext dbContext)
    : IQueryHandler<GetAllLabTestsQuery, IEnumerable<LabTestDto>>
{
    public async Task<BaseResponse<IEnumerable<LabTestDto>>> HandleAsync(GetAllLabTestsQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext.LabTests.AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            var search = query.SearchKey.ToLower();
            innerQuery = innerQuery.Where(x => x.Name.ToLower().Contains(search) || x.Description.ToLower().Contains(search));
        }

        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;

        var pagedList = await innerQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LabTestDto(x.Id.ToString(), x.Name, x.Description))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<LabTestDto>>
        {
            IsSuccess = true,
            Data = pagedList
        };
    }
}