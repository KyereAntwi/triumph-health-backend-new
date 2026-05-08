namespace Triumph.HealthMs.Core.Interfaces;

public interface IFacilityManagementDbContext
{
    DbSet<OrganizationalFacility> OrganizationalFacilities { get; }
    DbSet<FacilityManager> FacilityManagers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}