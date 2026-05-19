namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class TenantSubscriptionConfigurations : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.ToTable("TenantSubscriptions");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Tenant)
            .WithMany(t => t.TenantSubscriptions);

        builder.HasOne(x => x.Subscription)
            .WithMany();
        
        builder.Property(x => x.SubscriptionChargeRate)
            .IsRequired()
            .HasDefaultValue(SubscriptionChargeRate.Monthly);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}