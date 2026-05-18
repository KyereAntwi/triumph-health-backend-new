namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PermissionConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PermissionType)
            .HasDefaultValue(PermissionType.None)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(x => x.DisplayName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.PermissionType);
        builder.HasIndex(x => x.Deleted);
    }
}