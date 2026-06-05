namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class FacilityAnnouncementConfigurations : IEntityTypeConfiguration<FacilityAnnouncement>
{
    public void Configure(EntityTypeBuilder<FacilityAnnouncement> builder)
    {
        builder.ToTable("FacilityAnnouncements");
        builder.HasKey(fa => fa.Id);
        builder.Property(fa => fa.FacilityId).IsRequired();
    
        builder.HasOne(fa => fa.OrganizationalFacility)
            .WithMany(f => f.FacilityAnnouncements)
            .HasForeignKey(fa => fa.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(a => a.Message).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.Type).IsRequired();
        builder.Property(a => a.ValidUntil).IsRequired();
        builder.HasIndex(a => new { a.TenantId, a.FacilityId, a.Deleted, a.Type, a.CreatedAt });
        builder.HasIndex(a => a.ValidUntil);
    }
}