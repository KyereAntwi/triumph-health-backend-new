namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class VisitationConfigurations : IEntityTypeConfiguration<Visitation>
{
    public void Configure(EntityTypeBuilder<Visitation> builder)
    {
        builder.ToTable("Visitations");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.VisitingReason)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.PatientId, x.Deleted });

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Visitations)
            .HasForeignKey(x => x.PatientId);

        builder.HasMany(x => x.PatientVitals)
            .WithOne(x => x.Visitation)
            .HasForeignKey(x => x.VisitationId);

        builder.HasMany(x => x.Consultations)
            .WithOne(x => x.Visitation)
            .HasForeignKey(x => x.VisitationId);

        builder.HasMany(x => x.PatientLabTests)
            .WithOne(x => x.Visitation)
            .HasForeignKey(x => x.VisitationId);
    }
}