namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PatientHealthDiagnosisConfigurations : IEntityTypeConfiguration<PatientHealthDiagnosis>
{
    public void Configure(EntityTypeBuilder<PatientHealthDiagnosis> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExtraNotes)
            .HasMaxLength(500);
        builder.Property(x => x.DiagnosedByFullname)
            .HasMaxLength(50);
        builder.Property(x => x.HealthFacilityDiagnosedAt)
            .HasMaxLength(100);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId);

        builder.HasOne(x => x.HealthDiagnosis)
            .WithMany()
            .HasForeignKey(x => x.HealthDiagnosisId);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.PatientId, x.HealthDiagnosisId, x.Deleted });
    }
}