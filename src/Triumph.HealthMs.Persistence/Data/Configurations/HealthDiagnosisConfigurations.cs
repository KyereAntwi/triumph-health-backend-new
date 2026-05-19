namespace Triumph.HealthMs.Persistence.Data.Configurations;

public class HealthDiagnosisConfigurations : IEntityTypeConfiguration<HealthDiagnosis>
{
    public void Configure(EntityTypeBuilder<HealthDiagnosis> builder)
    {
        builder.ToTable("HealthDiagnoses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.RecommendedPrescription)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.Deleted, x.Name });
    }
}