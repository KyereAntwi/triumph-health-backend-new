namespace Triumph.HealthMs.Core.Interfaces;

public interface IFacilityManagementDbContext
{
    DbSet<OrganizationalFacility> OrganizationalFacilities { get; }
    DbSet<FacilityManager> FacilityManagers { get; }
    DbSet<FacilityLabTest> FacilityLabTests { get; }
    DbSet<FacilityAnnouncement> FacilityAnnouncements { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}