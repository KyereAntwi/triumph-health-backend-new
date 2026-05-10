namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmployeeRoleConfigurations : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);
        
        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.EmployeeId, x.Deleted, x.RoleId });
        
        builder.Property(x => x.StartedFrom)
            .IsRequired();
    }
}