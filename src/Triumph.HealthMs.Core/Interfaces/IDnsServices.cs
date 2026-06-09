namespace Triumph.HealthMs.Core.Interfaces;

public interface IDnsServices
{
    Task<bool> CreateSubdomain(string subdomain, CancellationToken cancellationToken = default);
    Task DeleteSubdomain(string subdomain, CancellationToken cancellationToken = default);
}