namespace Triumph.HealthMs.Core.Interfaces;

public interface IPatientManagementDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Identification> Identifications { get; }
    DbSet<Consultation> Consultations { get; }
    DbSet<PatientDrug> PatientDrugs { get; }
    DbSet<PatientHealthDiagnosis> PatientHealthDiagnoses { get; }
    DbSet<PatientLabTest> PatientLabTests { get; }
    DbSet<PatientVital> PatientVitals { get; }
    DbSet<Visitation> Visitations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}