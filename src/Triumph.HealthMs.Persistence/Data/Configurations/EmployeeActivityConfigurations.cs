namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmployeeActivityConfigurations : IEntityTypeConfiguration<EmployeeActivity>
{
    public void Configure(EntityTypeBuilder<EmployeeActivity> builder)
    {
        builder.ToTable("EmployeeActivities");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.EmployeeId, x.Deleted });

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);
    }
}