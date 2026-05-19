namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PatientLabTestConfigurations : IEntityTypeConfiguration<PatientLabTest>
{
    public void Configure(EntityTypeBuilder<PatientLabTest> builder)
    {
        builder.ToTable("PatientLabTests");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
            { x.TenantId, x.FacilityId, x.VisitationId, x.SupervisedById, x.LabTestId, x.RecommendedById, x.Deleted });

        builder.Property(x => x.ExtraNotes)
            .HasMaxLength(500);

        builder.Property(x => x.MeasuredValue)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.SupervisedById)
            .IsRequired();
        builder.Property(x => x.RecommendedById)
            .IsRequired();

        builder.HasOne(x => x.LabTest)
            .WithMany()
            .HasForeignKey(x => x.LabTestId);

        builder.HasOne(x => x.Visitation)
            .WithMany(x => x.PatientLabTests)
            .HasForeignKey(x => x.VisitationId);
    }
}