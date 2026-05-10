namespace Triumph.HealthMs.Core.Interfaces;

public interface IUpsetEmployeeService
{
    Task<(string, Guid?, Guid?)> UpsetEmployeeDetailsAsync(AddAnEmployeeCommand command, CancellationToken cancellationToken);

    Task<string?> UpdateEmployeePermissionsAsync(Employee employee, UpdateEmployeePermissionsCommand command,
        CancellationToken cancellationToken);
}