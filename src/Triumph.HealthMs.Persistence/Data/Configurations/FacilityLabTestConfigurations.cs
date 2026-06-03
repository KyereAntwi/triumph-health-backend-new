namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class FacilityLabTestConfigurations : IEntityTypeConfiguration<FacilityLabTest>
{
    public void Configure(EntityTypeBuilder<FacilityLabTest> builder)
    {
        builder.ToTable("FacilityLabTests");
        builder.HasIndex(x => x.Id);
        
        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.AdditionalFacilityNotes)
            .HasMaxLength(500);

        builder.Property(x => x.UniqueIdentifier)
            .IsRequired()
            .HasMaxLength(7);

        builder.HasOne(x => x.Facility)
            .WithMany()
            .HasForeignKey(x => x.FacilityId);

        builder.HasOne(x => x.LabTest)
            .WithMany()
            .HasForeignKey(x => x.LabTestId);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.Deleted });
        builder.HasIndex(x => x.LabTestId);
        builder.HasIndex(x => x.UniqueIdentifier);
    }
}