namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class TenantManagerConfiguration : IEntityTypeConfiguration<TenantManager>
{
    public void Configure(EntityTypeBuilder<TenantManager> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.TenantManagers)
            .HasForeignKey(x => x.TenantId);

        builder.HasIndex(x => new {x.TenantId, x.ApplicationUserId});
        builder.Property(x => x.ApplicationUserId)
            .IsRequired();
    }
}