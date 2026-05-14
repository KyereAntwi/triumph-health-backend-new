namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class ConsultationConfigurations : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.HasIndex(x => x.Id);

        builder.HasOne(x => x.Visitation)
            .WithMany(x => x.Consultations)
            .HasForeignKey(x => x.VisitationId);

        builder.Property(x => x.Notes)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Room)
            .HasMaxLength(10);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.Deleted });
        builder.HasIndex(x => x.VisitationId);
    }
}