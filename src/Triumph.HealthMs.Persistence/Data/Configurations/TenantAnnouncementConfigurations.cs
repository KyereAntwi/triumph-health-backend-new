namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class TenantAnnouncementConfigurations : IEntityTypeConfiguration<TenantAnnouncement>
{
    public void Configure(EntityTypeBuilder<TenantAnnouncement> builder)
    {
        builder.ToTable("TenantAnnouncements");
        builder.HasKey(ta => ta.Id);
        builder.Property(ta => ta.TenantId).IsRequired();
    
        builder.HasOne(ta => ta.Tenant)
            .WithMany(ta => ta.TenantAnnouncements)
            .HasForeignKey(ta => ta.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(a => a.Message).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.Type).IsRequired();
        builder.Property(a => a.ValidUntil).IsRequired();
        builder.HasIndex(a => new { a.TenantId, a.Deleted, a.Type, a.CreatedAt });
        builder.HasIndex(a => a.ValidUntil);
    }
}