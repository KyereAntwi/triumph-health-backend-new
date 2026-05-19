namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PatientVitalConfigurations : IEntityTypeConfiguration<PatientVital>
{
    public void Configure(EntityTypeBuilder<PatientVital> builder)
    {
        builder.ToTable("PatientVitals");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.VitalItem)
            .WithMany()
            .HasForeignKey(x => x.VitalItemId);

        builder.HasOne(x => x.Visitation)
            .WithMany()
            .HasForeignKey(x => x.VisitationId);

        builder.Property(x => x.MeasurementValue)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.Deleted });
        builder.HasIndex(x => new { x.VisitationId, x.VitalItemId });
    }
}