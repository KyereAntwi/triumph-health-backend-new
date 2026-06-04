namespace Triumph.HealthMs.Core.Features.General.GetAllHealthDiagnosis;

public sealed class GetAllHealthDiagnosisQueryHandler(
    ICommonEntitiesDbContext dbContext) 
    : IQueryHandler<GetAllHealthDiagnosisQuery, IEnumerable<HealthDiagnosisDto>>
{
    public async Task<BaseResponse<IEnumerable<HealthDiagnosisDto>>> HandleAsync(GetAllHealthDiagnosisQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext.HealthDiagnoses.AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchKey))
        {
            var search = query.SearchKey.ToLower();
            
            innerQuery = innerQuery.Where(
                hd => hd.Name.ToLower().Contains(search) || 
                      hd.Description.ToLower().Contains(search));
        }
        
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        
        var pagedList = await innerQuery
            .OrderByDescending(hd => hd.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(hd => new HealthDiagnosisDto(hd.Id.ToString(), hd.Name, hd.Description, hd.RecommendedPrescription ?? string.Empty))
            .ToArrayAsync(cancellationToken);

        return new BaseResponse<IEnumerable<HealthDiagnosisDto>>
        {
            IsSuccess = true,
            Data = pagedList
        };
    }
}