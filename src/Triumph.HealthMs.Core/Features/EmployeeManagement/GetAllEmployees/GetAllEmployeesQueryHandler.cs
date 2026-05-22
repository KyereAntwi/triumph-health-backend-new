namespace Triumph.HealthMs.Core.Features.EmployeeManagement.GetAllEmployees;

public sealed class GetAllEmployeesQueryHandler(
    IApplicationUserManagementDbContext appUserDbContext,
    IEmployeeManagementDbContext empDbContext) 
    : IQueryHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
{
    public async Task<BaseResponse<IEnumerable<EmployeeDto>>> HandleAsync(GetAllEmployeesQuery query, CancellationToken cancellationToken = default)
    {
        var employeesQuery = empDbContext.Employees.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(query.EmployeeId))
        {
            var employeeGuid = Guid.Parse(query.EmployeeId);
            employeesQuery = employeesQuery
                .Where(e => e.Id == employeeGuid);
        }
        
        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            employeesQuery = employeesQuery
                .Where(e => e.EmployeeRoles
                    .Any(r => r.Role!.Title == query.Role));
        }
        
        if (query.MonthOfBirth > 0)
        {
            var temporalUsers = appUserDbContext.ApplicationUsers
                .Where(u => u.DateOfBirth.Month == query.MonthOfBirth)
                .Select(u => u.Id);
            
            employeesQuery = employeesQuery.Where(e => temporalUsers.Contains(e.ApplicationUserId));
        }
        
        if (!string.IsNullOrWhiteSpace(query.SearchKey))
        {
            var search = query.SearchKey.ToLower();

            var temporalUsers = appUserDbContext.ApplicationUsers
                .Where(u => u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search))
                .Select(u => u.Id);

            employeesQuery = employeesQuery.Where(e => temporalUsers.Contains(e.ApplicationUserId));
        }
        
        var pageSize = query.PageSize > 50 ? 50 : query.PageSize;
        
        var employees = await employeesQuery
            .OrderByDescending(e => e.CreatedAt)
            .Skip((query.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                e.Id,
                e.ApplicationUserId,
                e.EmployedAt
            })
            .ToArrayAsync(cancellationToken);
        
        var employeeIds = employees.Select(x => x.Id).ToList();
        var userIds = employees.Select(x => x.ApplicationUserId).ToList();

        var users = await appUserDbContext.ApplicationUsers
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.OtherNames,
                u.Gender,
                u.Nationality,
                u.DateOfBirth,
                u.Email,
                u.PhoneNumber,
                u.ProfileImageUrl
            })
            .ToDictionaryAsync(u => u.Id, cancellationToken);
        
        Dictionary<Guid, List<EmployeeRoleDto>>? roles = null;
        if (query.IncludeRoles)
        {
            roles = await empDbContext.EmployeeRoles
                .Where(r => employeeIds.Contains(r.EmployeeId))
                .Select(r => new
                {
                    r.EmployeeId,
                    Role = new EmployeeRoleDto(
                        r.Id.ToString(),
                        r.Role!.Title,
                        r.Role.Description,
                        r.StartedFrom.ToShortDateString(),
                        r.EndedAt.ToString())
                })
                .GroupBy(x => x.EmployeeId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(x => x.Role).ToList(),
                    cancellationToken);
        }

        Dictionary<Guid, List<EmployeeActivityDto>>? activities = null;
        if (query.IncludeActivities)
        {
            activities = await empDbContext.EmployeeActivities
                .Where(a => employeeIds.Contains(a.EmployeeId))
                .Select(a => new
                {
                    a.EmployeeId,
                    Activity = new EmployeeActivityDto(
                        a.Id.ToString(),
                        a.Action,
                        a.CreatedAt.ToString())
                })
                .GroupBy(x => x.EmployeeId)
                .ToDictionaryAsync(g => g.Key,
                    g => g.Select(x => x.Activity).ToList(),
                    cancellationToken);
        }

        Dictionary<Guid, List<EmployeeAttachmentDto>>? attachments = null;
        if (query.IncludeAttachments)
        {
            attachments = await empDbContext.EmploymentAttachments
                .Where(a => employeeIds.Contains(a.EmployeeId))
                .Select(a => new
                {
                    a.EmployeeId,
                    Attachment = new EmployeeAttachmentDto(
                        a.Id.ToString(),
                        a.AttachmentUrl,
                        a.AttachmentType)
                })
                .GroupBy(x => x.EmployeeId)
                .ToDictionaryAsync(g => g.Key,
                    g => g.Select(x => x.Attachment).ToList(),
                    cancellationToken);
        }

        Dictionary<Guid, List<string>>? permissions = null;
        if (query.IncludePermissions)
        {
            permissions = await empDbContext.EmployeePermissions
                .Where(p => employeeIds.Contains(p.EmployeeId))
                .Select(p => new
                {
                    p.EmployeeId,
                    Permission = p.Permission!.PermissionType.ToString()
                })
                .GroupBy(x => x.EmployeeId)
                .ToDictionaryAsync(g => g.Key,
                    g => g.Select(x => x.Permission).ToList(),
                    cancellationToken);
        }
        
        var result = employees.Select(e =>
        {
            users.TryGetValue(e.ApplicationUserId, out var user);

            return new EmployeeDto
            {
                Id = e.Id.ToString(),

                FirstName = user!.FirstName,
                LastName = user.LastName,
                Gender = user.Gender.ToString(),
                Nationality = user.Nationality.ToString(),
                DateOfBirth = user.DateOfBirth.ToShortDateString(),
                OtherNames = user.OtherNames,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,

                EmployedAt = e.EmployedAt?.ToString(),

                Roles = query.IncludeRoles 
                        && roles != null 
                        && roles.TryGetValue(e.Id, out var r) ? r : [],

                Permissions = query.IncludePermissions
                            && permissions != null
                            && permissions.TryGetValue(e.Id, out var permissionList) ? permissionList :[],
                
                Attachments = query.IncludeAttachments 
                              && attachments != null 
                              && attachments.TryGetValue(e.Id, out var attachmentList) ? attachmentList :[],
                
                Activities = query.IncludeActivities 
                             && activities != null 
                             && activities.TryGetValue(e.Id, out var activityList) ? activityList : []
            };
        });

        return new BaseResponse<IEnumerable<EmployeeDto>>
        {
            IsSuccess = true,
            Data = result
        };
    }
}