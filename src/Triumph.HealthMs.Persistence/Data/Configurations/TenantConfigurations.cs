namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class TenantConfigurations : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UniqueIdentifier)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasIndex(x => x.UniqueIdentifier)
            .IsUnique();

        builder.HasMany(x => x.TenantSubscriptions)
            .WithOne(ts => ts.Tenant)
            .HasForeignKey(ts => ts.TenantId);
    }
}