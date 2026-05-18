namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class VitalItemConfigurations : IEntityTypeConfiguration<VitalItem>
{
    public void Configure(EntityTypeBuilder<VitalItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => new {x.Deleted, x.Name});
    }
}