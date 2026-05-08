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

        builder.Property(x => x.OrganizationTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.MainTelephone)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasMany(x => x.TenantSubscriptions)
            .WithOne(ts => ts.Tenant)
            .HasForeignKey(ts => ts.TenantId);
    }
}