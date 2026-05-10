namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmployeePermissionConfigurations : IEntityTypeConfiguration<EmployeePermission>
{
    public void Configure(EntityTypeBuilder<EmployeePermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

        builder.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.EmployeeId, x.Deleted });
    }
}