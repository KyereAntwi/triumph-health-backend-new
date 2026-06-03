namespace Triumph.HealthMs.Core.Features.TenantManagement.GetDepartments;

public sealed class GetDepartmentsQueryHandler(IEmployeeManagementDbContext dbContext) 
    : IQueryHandler<object, IEnumerable<DepartmentDto>>
{
    public async Task<BaseResponse<IEnumerable<DepartmentDto>>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var list =  await dbContext
            .Departments
            .Select(d => new DepartmentDto(
                d.Id.ToString(), 
                d.Name, 
                d.Description ?? string.Empty))
            .ToArrayAsync(cancellationToken);
        
        return new BaseResponse<IEnumerable<DepartmentDto>>
        {
            IsSuccess = true,
            Data = list
        };
    }
}