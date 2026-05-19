namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmploymentAttachmentConfigurations : IEntityTypeConfiguration<EmploymentAttachment>
{
    public void Configure(EntityTypeBuilder<EmploymentAttachment> builder)
    {
        builder.ToTable("EmploymentAttachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttachmentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.AttachmentUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.EmployeeId, x.Deleted });
    }
}