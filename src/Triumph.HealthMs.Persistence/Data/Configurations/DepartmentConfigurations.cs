namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class DepartmentConfigurations : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.HasIndex(x => new { x.Name, x.Deleted });
        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Employees)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId);
    }
}