namespace Triumph.HealthMs.Persistence.Services;

public class UpsetEmployeeService(
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
    IEmployeeManagementDbContext dbContext,
    NpgsqlConnection connection,
    IFacilityManagementDbContext facilityManagementDbContext,
    ILogger<UpsetEmployeeService> logger)
    : IUpsetEmployeeService
{
    public async Task<(string, Guid?, Guid?)> UpsetEmployeeDetailsAsync(AddAnEmployeeCommand command, CancellationToken cancellationToken)
    {
        var permissions = command.Permissions != null && command.Permissions.Any()
            ? command.Permissions.Select(p => Enum.Parse<PermissionType>(p)).ToList()
            : [];

        var permissionIds = await dbContext.Permissions
            .Where(p => permissions.Contains(p.PermissionType))
            .Select(p => new { p.Id, p.PermissionType })
            .ToArrayAsync(cancellationToken);

        var applicationUser = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid().ToString(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Gender = Enum.Parse<Gender>(command.Gender),
            Nationality = Enum.Parse<Nationality>(command.Nationality),
            DateOfBirth = DateOnly.Parse(command.DateOfBirth),
            OtherNames = command.OtherNames,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber
        };
        
        var invitation = new LinkInvitation
        {
            Id = Guid.CreateVersion7(),
            ApplicationUserId = applicationUser.Id,
            InvitedEntityType = nameof(Employee)
        };

        var newEmployee = new Employee
        {
            Id = Guid.CreateVersion7(),
            ApplicationUserId = applicationUser.Id,
            EmployedAt = DateOnly.Parse(command.EmployedAt),
            FacilityId = Guid.Parse(command.FacilityId)
        };
        
        newEmployee.EmployeeRoles.Add(
            new EmployeeRole
            {
                RoleId = Guid.Parse(command.RoleId),
                EmployeeId = newEmployee.Id,
                FacilityId = Guid.Parse(command.FacilityId)
            });
        
        foreach (var permission in permissionIds)
        {
            newEmployee.EmployeePermissions.Add(new EmployeePermission
            {
                EmployeeId = newEmployee.Id,
                PermissionId = permission.Id,
                FacilityId = Guid.Parse(command.FacilityId)
            });
        }
        
        var strategy = ((DbContext)dbContext).Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

                await using var tx = await connection.BeginTransactionAsync(cancellationToken);
                
                await ((DbContext)applicationUserManagementDbContext).Database.UseTransactionAsync(tx, cancellationToken);
                await ((DbContext)facilityManagementDbContext).Database.UseTransactionAsync(tx, cancellationToken);
                await ((DbContext)dbContext).Database.UseTransactionAsync(tx, cancellationToken);
                
                await applicationUserManagementDbContext.ApplicationUsers.AddAsync(applicationUser, cancellationToken);
                await applicationUserManagementDbContext.LinkInvitations.AddAsync(invitation, cancellationToken);
                await applicationUserManagementDbContext.SaveChangesAsync(cancellationToken);

                if (command.SetAsFacilityManager)
                {
                    var manager = new FacilityManager
                    {
                        FacilityId = Guid.Parse(command.FacilityId),
                        ApplicationUserId = applicationUser.Id
                    };
                    await facilityManagementDbContext.FacilityManagers.AddAsync(manager, cancellationToken);
                    await facilityManagementDbContext.SaveChangesAsync(cancellationToken);
                }

                await dbContext.Employees.AddAsync(newEmployee, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to persist employee after execution strategy retries. Command: {@Command}", command);
            return ("An error occurred while adding the employee.", null, null);
        }
        finally
        {
            await connection.CloseAsync();
        }

        return (string.Empty, newEmployee.Id, invitation.Id);
    }

    public async Task<string?> UpdateEmployeePermissionsAsync(Employee employee, UpdateEmployeePermissionsCommand command,
        CancellationToken cancellationToken)
    {
        var permissions = command.Permissions.Select(p => Enum.Parse<PermissionType>(p)).ToArray();
        
        var permissionIds = await dbContext.Permissions
            .Where(p => permissions.Contains(p.PermissionType))
            .Select(p => p.Id)
            .ToArrayAsync(cancellationToken);

        if (permissionIds.Length != permissions.Length)
            logger.LogWarning(
                "Some permissions in the command were not found in the database. Requested: {Requested}, Found: {Found}",
                permissions.Length, permissionIds.Length);

        var strategy = ((DbContext)dbContext).Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

                await using var tx = await connection.BeginTransactionAsync(cancellationToken);
                await ((DbContext)dbContext).Database.UseTransactionAsync(tx, cancellationToken);

                await dbContext
                    .EmployeePermissions
                    .Where(ep => ep.EmployeeId == employee.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                var employeePermissions = permissionIds.Select(id => new EmployeePermission
                {
                    EmployeeId = employee.Id,
                    PermissionId = id,
                    FacilityId = employee.FacilityId
                });

                await dbContext.EmployeePermissions.AddRangeAsync(employeePermissions, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);
            });
            
            return null;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update employee permissions after execution strategy retries. Command: {@Command}", command);
            return "An error occurred while updating employee permissions.";
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}