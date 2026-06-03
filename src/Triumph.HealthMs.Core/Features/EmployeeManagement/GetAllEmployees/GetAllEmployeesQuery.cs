namespace Triumph.HealthMs.Core.Features.EmployeeManagement.GetAllEmployees;

public record GetAllEmployeesRequest(
    string EmployeeId = "",
    string SearchKey = "",
    string RoleId = "",
    string DepartmentId = "",
    int MonthOfBirth = 0,
    int Page = 1,
    int PageSize = 10);

public record GetAllEmployeesQuery(
    string EmployeeId,
    string SearchKey,
    string RoleId,
    string DepartmentId,
    int MonthOfBirth,
    int Page,
    int PageSize,
    bool IncludeRoles,
    bool IncludePermissions,
    bool IncludeAttachments,
    bool IncludeActivities);

public record EmployeeDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string? OtherNames { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? EmployedAt { get; set; }
    public IEnumerable<EmployeeRoleDto> Roles { get; set; } = [];
    public IEnumerable<string> Permissions { get; set; } = [];
    public IEnumerable<EmployeeAttachmentDto> Attachments { get; set; } = [];
    public IEnumerable<EmployeeActivityDto> Activities { get; set; } = [];
}

public record EmployeeDtoForQuerying
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public DateOnly? EmployedAt { get; set; }
    public IEnumerable<EmployeeRoleDto> Roles { get; set; } = [];
    public IEnumerable<string> Permissions { get; set; } = [];
    public IEnumerable<EmployeeAttachmentDto> Attachments { get; set; } = [];
    public IEnumerable<EmployeeActivityDto> Activities { get; set; } = [];
}

public record EmployeeRoleDto(
    string RoleId, 
    string Title, 
    string? Description, 
    string StartedFrom, 
    string? EndedAt);
public record EmployeeAttachmentDto(
    string AttachmentId, 
    string AttachmentUrl, 
    string AttachmentType);
public record EmployeeActivityDto(
    string ActivityId, 
    string Action, 
    string? CreatedAt);