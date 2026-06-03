namespace Triumph.HealthMs.Core.Interfaces;

public interface IEmployeeManagementDbContext
{
    DbSet<Employee> Employees { get; }
    DbSet<EmployeeActivity> EmployeeActivities { get; }
    DbSet<EmployeePermission> EmployeePermissions { get; }
    DbSet<EmployeeRole> EmployeeRoles { get; }
    public DbSet<EmploymentAttachment> EmploymentAttachments { get; }
    public DbSet<Permission> Permissions { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<Department> Departments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}