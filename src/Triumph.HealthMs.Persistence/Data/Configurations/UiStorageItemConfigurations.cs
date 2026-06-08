namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class UiStorageItemConfigurations : IEntityTypeConfiguration<UiStorageItem>
{
    public void Configure(EntityTypeBuilder<UiStorageItem> builder)
    {
        builder.ToTable("UiStorageItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(2000);

        builder.HasIndex(x => new { x.Key, x.CreatedBy, x.Deleted });
    }
}