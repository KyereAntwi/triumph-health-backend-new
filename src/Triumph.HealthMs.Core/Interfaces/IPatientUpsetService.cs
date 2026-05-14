namespace Triumph.HealthMs.Core.Interfaces;

public interface IPatientUpsetService
{
    Task<(string?, Guid?)> UpsetPatientDetails(AddPatientCommand command, CancellationToken cancellationToken);
}