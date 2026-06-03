namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApplicationUserId)
            .IsRequired();
        
        builder.HasIndex(x => x.ApplicationUserId)
            .IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.Deleted });

        builder.HasMany(x => x.EmployeeRoles)
            .WithOne(r => r.Employee)
            .HasForeignKey(r => r.EmployeeId);

        builder.HasMany(x => x.EmployeePermissions)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);

        builder.HasMany(x => x.EmploymentAttachments)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.EmployeeActivities)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId);

        builder.HasIndex(x => x.DepartmentId);

        builder.Property(x => x.UniqueIdentifier)
            .HasMaxLength(10);
        builder.HasIndex(x => x.UniqueIdentifier);
    }
}