namespace Triumph.HealthMs.Core.Features.General.GetAllDrugs;

public sealed class GetAllDrugsQueryHandler (
    ICommonEntitiesDbContext dbContext)
    : IQueryHandler<GetAllDrugsQuery, IEnumerable<DrugDto>>
{
    public async Task<BaseResponse<IEnumerable<DrugDto>>> HandleAsync(GetAllDrugsQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext.Drugs.AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            var search = query.SearchKey.ToLower();
            
            innerQuery = innerQuery
                .Where(d => 
                    d.Name.ToLower().Contains(search) || 
                    d.Description.ToLower().Contains(search) ||
                    d.Prescription.ToLower().Contains(search));
        }
        
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;

        var pagedList = await innerQuery
            .OrderByDescending(d => d.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DrugDto(
                Id: d.Id.ToString(),
                Name: d.Name,
                Description: d.Description,
                Prescription: d.Prescription,
                Manufacturer: d.Manufacturer ?? string.Empty))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<DrugDto>>
        {
            IsSuccess = true,
            Data = pagedList
        };
    }
}