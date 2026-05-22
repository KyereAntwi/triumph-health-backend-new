namespace Triumph.HealthMs.Core.Interfaces;

public interface ICommonEntitiesDbContext
{
    DbSet<Drug> Drugs { get; }
    DbSet<VitalItem> VitalItems { get; }
    DbSet<HealthDiagnosis> HealthDiagnoses { get; }
    DbSet<LabTest> LabTests { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}