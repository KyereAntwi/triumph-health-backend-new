namespace Triumph.HealthMs.Queries.QueryTypes;

[Authorize]
[ExtendObjectType<QueryBase>]
public class EmployeesQueries
{
    public async Task<IEnumerable<EmployeeDto>> GetAllEmployees(
        GetAllEmployeesRequest? request,
        IResolverContext context,
        ILoggedInUserService loggedInUserService,
        IPermissionService permissionService,
        IQueryHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>> handler,
        IEmployeeManagementDbContext employeeManagementDbContext,
        IApplicationUserManagementDbContext applicationUserManagementDbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loggedInUserService.TenantId))
            throw new GraphQLRequestException("Tenant Id missing");

        if (!await permissionService.HasActiveSubscription(cancellationToken))
            throw new GraphQLRequestException("You do not have an active subscription");

        var includeRoles = context.IsSelected("roles");
        var includePermissions = context.IsSelected("permissions");
        var includeAttachments = context.IsSelected("attachments");
        var includeActivities = context.IsSelected("activities");

        var query = new GetAllEmployeesQuery(
            request?.EmployeeId ?? string.Empty,
            request?.SearchKey ?? string.Empty,
            request?.RoleId ?? string.Empty,
            request?.DepartmentId ?? string.Empty,
            request?.MonthOfBirth ?? 0,
            request?.Page ?? 1,
            request?.PageSize ?? 10,
            includeRoles,
            includePermissions,
            includeAttachments,
            includeActivities);
        
        await ConfirmAccessToEmployeeDetails(query, 
            loggedInUserService, 
            permissionService, 
            employeeManagementDbContext, 
            applicationUserManagementDbContext, 
            cancellationToken);
        
        var result = await handler.HandleAsync(
            query, 
            cancellationToken);

        return result.Data!;
    }

    private static async Task ConfirmAccessToEmployeeDetails(
        GetAllEmployeesQuery query,
        ILoggedInUserService loggedInUserService, 
        IPermissionService permissionService,
        IEmployeeManagementDbContext employeeManagementDbContext,
        IApplicationUserManagementDbContext applicationUserManagementDbContext, CancellationToken cancellationToken)
    {
        if (query is { IncludeActivities: false, IncludeAttachments: false, IncludePermissions: false, IncludeRoles: false })
            return;
        
        if (await permissionService.UserIsAManager(cancellationToken)) return;
        
        var loggedInUser = await 
            applicationUserManagementDbContext.ApplicationUsers.FirstOrDefaultAsync(u =>
                u.UserId == loggedInUserService.UserId, cancellationToken);

        var employee =
            await employeeManagementDbContext.Employees.FirstOrDefaultAsync(e =>
                e.ApplicationUserId == loggedInUser!.Id, cancellationToken);
            
        if (employee is null)
            throw new GraphQLRequestException("You do not have permission to view this employee details");
    }
}