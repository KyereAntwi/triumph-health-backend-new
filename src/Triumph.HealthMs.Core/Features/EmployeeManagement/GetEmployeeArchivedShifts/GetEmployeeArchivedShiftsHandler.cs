namespace Triumph.HealthMs.Core.Features.EmployeeManagement.GetEmployeeArchivedShifts;

public sealed class GetEmployeeArchivedShiftsQueryHandler(
    IEmployeeManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService) 
    : IQueryHandler<GetEmployeeArchivedShiftsQuery, IEnumerable<ArchivedShiftDto>>
{
    public async Task<BaseResponse<IEnumerable<ArchivedShiftDto>>> HandleAsync(GetEmployeeArchivedShiftsQuery query, CancellationToken cancellationToken = default)
    {
        var innerQuery = dbContext
            .EmployeeShifts
            .IgnoreQueryFilters()
            .Where(es => 
                es.EmployeeId == Guid.Parse(query.EmployeeId) 
                && !es.Deleted 
                && es.TenantId == Guid.Parse(loggedInUserService.TenantId!))
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.From) && DateTime.TryParse(query.From, out var fromDate))
        {
            innerQuery = innerQuery.Where(es => es.CreatedAt!.Value.Date >= fromDate.Date);
        }

        if (!string.IsNullOrEmpty(query.To) && DateTime.TryParse(query.To, out var toDate))
        {
            innerQuery = innerQuery.Where(es => es.CreatedAt!.Value.Date <= toDate.Date);
        }
        
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        var shifts = await innerQuery
            .OrderByDescending(s => s.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(es => new ArchivedShiftDto(
                es.Id.ToString(),
                es.StartedAt!.Value.ToShortTimeString(),
                es.EndedAt!.Value.ToShortTimeString(),
                es.ShiftType.ToString(),
                es.TimeStamp.ToShortDateString(),
                es.CreatedBy.ToString(),
                ConvertIntToDayOfWeek.Convert(es.DayOfWeek),
                es.ShiftDurationInHours))
            .ToArrayAsync<ArchivedShiftDto>(cancellationToken);

        return new BaseResponse<IEnumerable<ArchivedShiftDto>>
        {
            IsSuccess = true,
            Message = "Archived shifts retrieved successfully",
            Data = shifts
        };
    }
}