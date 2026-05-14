namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PatientConfigurations : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.Deleted });

        builder.Property(x => x.UniqueIdentifier)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PostGps)
            .HasMaxLength(13);

        builder.HasMany(x => x.Identifications)
            .WithOne(x => x.Patient)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PatientHealthDiagnoses)
            .WithOne()
            .HasForeignKey(x => x.HealthDiagnosisId);

        builder.HasMany(x => x.Visitations)
            .WithOne(x => x.Patient)
            .HasForeignKey(x => x.PatientId);

        builder.HasMany(x => x.PatientDrugs)
            .WithOne(x => x.Patient)
            .HasForeignKey(x => x.PatientId);
    }
}