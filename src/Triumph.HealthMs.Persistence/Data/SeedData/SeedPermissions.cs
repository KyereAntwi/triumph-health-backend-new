namespace Triumph.HealthMs.Persistence.Data.SeedData;

public sealed class SeedPermissions : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasData([
            new Permission
            {
                Id = Guid.Parse("123e4567-e89b-12d3-a456-426655440000"),
                CreatedBy = "System",
                CreatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedBy = "System",
                Deleted = false,
                PermissionType = PermissionType.None,
                Description = "Permission type has no effect on any entity on the system",
                DisplayName = "None"
            },
            
            new Permission
            {
                Id = Guid.Parse("323e4567-e89b-12d3-a456-426655440000"),
                CreatedBy = "System",
                CreatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedBy = "System",
                Deleted = false,
                PermissionType = PermissionType.MANAGE_PATIENT_BIOGRAPHY,
                Description = "Create and update the biography of a registered Patient only.",
                DisplayName = "Manage Patient Biography"
            }
        ]);
    }
}