namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class DrugConfigurations : IEntityTypeConfiguration<Drug>
{
    public void Configure(EntityTypeBuilder<Drug> builder)
    {
        builder.ToTable("Drugs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.Prescription)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.Manufacturer)
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.Name, x.Deleted });
    }
}