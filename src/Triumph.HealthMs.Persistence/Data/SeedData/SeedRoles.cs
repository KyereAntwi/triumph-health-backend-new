namespace Triumph.HealthMs.Persistence.Data.SeedData;

public sealed class SeedRoles : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData([
            new Role
            {
                Id = Guid.Parse("223e4567-e89b-12d3-a456-426655440000"),
                CreatedBy = "System",
                CreatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedAt = DateTime.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedBy = "System",
                Deleted = false,
                Title = "General Nurse",
                Description = ""
            }
        ]);
    }
}